using FidgetSpace.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;      // Preferences Local storage
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;          // Stopwatch
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using static SQLite.TableMapping;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FidgetSpace;


namespace FidgetSpace.Models.ViewModels
{
    /// <summary>
    /// Core logic of the connect-the-dots game (MVVM ViewModel):
    /// - Generate the game board
    /// - Handle clicks
    /// - Determine matching and game completion
    /// </summary>
    public class ConnectDotsViewModel : INotifyPropertyChanged
    {
        // Point collection used for binding to CollectionView
        public ObservableCollection<ConnectDotsCell> Cells { get; set; }

        // Number of lines (adjust as needed)
        private int rows = 4;
        public int Rows
        {
            get => rows;
            set
            {
                if (rows != value)
                {
                    rows = value;
                    OnPropertyChanged();
                }
            }
        }

        //Number of columns(default: 6 columns)
        private int cols = 6;
        public int Cols
        {
            get => cols;
            set
            {
                if (cols != value)
                {
                    cols = value;
                    OnPropertyChanged();
                }
            }
        }

        // Timing-related fields
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _isTimerRunning;

        // Save total seconds (historical + currently completed games)
        private int totalTimePlayedSeconds;

        //Total time text for UI display
        private string totalTimePlayedDisplay;
        public string TotalTimePlayedDisplay
        {
            get => totalTimePlayedDisplay;
            set
            {
                if (totalTimePlayedDisplay != value)
                {
                    totalTimePlayedDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        // Time Remaining for This Round” for UI display
        private string sessionTimeDisplay;
        public string SessionTimeDisplay
        {
            get => sessionTimeDisplay;
            set
            {
                if (sessionTimeDisplay != value)
                {
                    sessionTimeDisplay = value;
                    OnPropertyChanged();
                }
            }
        }
        // Remaining available pairs (displayed at the top of the page)
        private int remainingPairs;
        public int RemainingPairs
        {
            get => remainingPairs;
            set
            {
                if (remainingPairs != value)
                {
                    remainingPairs = value;
                    OnPropertyChanged();
                }
            }
        }

        // Can the current board be further manipulated?
        private bool boardActive = true;
        public bool BoardActive
        {
            get => boardActive;
            set
            {
                if (boardActive != value)
                {
                    boardActive = value;
                    OnPropertyChanged();
                }
            }
        }

        // New Game Button Command
        public ICommand NewGameCommand { get; set; }

        // Command triggered when clicking a specific point
        public ICommand TapCommand { get; set; }

        private readonly Random random = new Random();
        private ConnectDotsCell firstSelected;   // Record the first selected point

        // Set of colors used
        private readonly string[] colors = new[]
        {
            "#FF6B6B", // red
            "#4D96FF", // blue
            "#6BCB77"  // green
        };

        public ConnectDotsViewModel()
        {
            Cells = new ObservableCollection<ConnectDotsCell>();

            NewGameCommand = new Command(GenerateBoard);
            TapCommand = new Command<ConnectDotsCell>(OnTap);

            // Initialize cumulative game time: prioritize reading from the currently logged-in user
            if (App.LoggedInUser != null)
            {
                // Read the previously accumulated DOT game time from the currently logged-in user object
                totalTimePlayedSeconds = App.LoggedInUser.TotalTimeDotSeconds;
            }
            else
            {
                // If no user is logged in, treat it as 0
                totalTimePlayedSeconds = 0;
            }

            // Total Time Display
            TotalTimePlayedDisplay = TimeSpan
                .FromSeconds(totalTimePlayedSeconds)
                .ToString(@"hh\:mm\:ss");

            // The game clock is initialized to 00:00:00
            SessionTimeDisplay = TimeSpan
                .FromSeconds(0)
                .ToString(@"hh\:mm\:ss");

            GenerateBoard();
        }


        /// <summary>
        /// Called when the page loads, starting the session timer (now triggered by the first click)
        /// </summary>
        public void StartTimer()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            _isTimerRunning = true;

            // Refreshes once per second TotalTimePlayedDisplay
            Application.Current.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (!_isTimerRunning)
                    return false; // Stop this UI timer

                // The number of seconds that have elapsed in this game
                int sessionSeconds = (int)_stopwatch.Elapsed.TotalSeconds;

                //Current Total Time = Total Seconds Saved + Seconds Elapsed in This Session
                int currentTotalSeconds = totalTimePlayedSeconds + sessionSeconds;

                // Total Update Time Display
                TotalTimePlayedDisplay = TimeSpan
                    .FromSeconds(currentTotalSeconds)
                    .ToString(@"hh\:mm\:ss");

                // Update the time display for this bureau
                SessionTimeDisplay = TimeSpan
                    .FromSeconds(sessionSeconds)
                    .ToString(@"hh\:mm\:ss");

                return true; // Return true to continue calling after 1 second (loop).
            });

        }

        /// <summary>
        /// Called when leaving the page, stops and saves the local time to the total duration.
        /// </summary>
        public void StopAndSaveTimer()
        {
            // If the timer never started, there's no need to save it
            if (!_isTimerRunning)
                return;

            _isTimerRunning = false;
            _stopwatch.Stop();

            int sessionSeconds = (int)_stopwatch.Elapsed.TotalSeconds;
            if (sessionSeconds <= 0)
                return;

            // Display the final time used for this game
            SessionTimeDisplay = TimeSpan
                .FromSeconds(sessionSeconds)
                .ToString(@"hh\:mm\:ss");

            // Add the current frame time to the total time (a variable in memory)
            totalTimePlayedSeconds += sessionSeconds;

            // If there is a currently logged-in user, write the time back to the User and database.
            if (App.LoggedInUser != null)
            {
                // Update the current user's total DOT game playtime
                App.LoggedInUser.TotalTimeDotSeconds = totalTimePlayedSeconds;

                // Simultaneously update the total game time (DOT + Bubble)
                App.LoggedInUser.TotalTimePlayedSeconds =
                    App.LoggedInUser.TotalTimeDotSeconds +
                    App.LoggedInUser.TotalTimeBubbleSeconds+
                    App.LoggedInUser.TotalTimePillSeconds;

                // Asynchronous database updates (without await)
                _ = App.Database.Update(App.LoggedInUser);
            }

            // Update the total time display once more to ensure it shows the latest value when stopped.
            TotalTimePlayedDisplay = TimeSpan
                .FromSeconds(totalTimePlayedSeconds)
                .ToString(@"hh\:mm\:ss");
        }




        /// <summary>
        /// Generate a new board: Colors appear in pairs, randomly distributed.
        /// </summary>
        void GenerateBoard()
        {
            Cells.Clear();
            firstSelected = null;
            BoardActive = true;

            int total = Rows * Cols;
            if (total % 2 != 0)
            {
                total -= 1; // Ensure it is an even number to facilitate pairing in pairs.
            }

            var colorList = new System.Collections.Generic.List<string>();
            int pairCount = total / 2;
            int colorCount = colors.Length;

            // Ensure each color roughly divides the total logarithm equally.
            int basePairs = pairCount / colorCount;
            int extraPairs = pairCount - basePairs * colorCount;

            for (int i = 0; i < colorCount; i++)
            {
                int pairs = basePairs + (i < extraPairs ? 1 : 0);
                for (int p = 0; p < pairs; p++)
                {
                    colorList.Add(colors[i]);
                    colorList.Add(colors[i]); // Two of the same color, forming a pair
                }
            }

            // Shuffle (disorder the color sequence)
            for (int i = colorList.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (colorList[i], colorList[j]) = (colorList[j], colorList[i]);
            }

            int id = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    int index = r * Cols + c;
                    if (index >= colorList.Count)
                        break;

                    var cell = new ConnectDotsCell
                    {
                        Id = id++,
                        Row = r,
                        Col = c,
                        ColorName = colorList[index],
                        IsVisible = true,
                        IsSelected = false
                    };

                    Cells.Add(cell);
                }
            }

            RemainingPairs = Cells.Count(x => x.IsVisible) / 2;
        }

        /// <summary>
        ///Handling logic when clicking a point:
        /// - First click: Select only
        /// - Second click: If colors match → Both disappear; otherwise toggle selection
        /// </summary>
        async void OnTap(ConnectDotsCell cell)
        {
            if (!BoardActive) return;
            if (cell == null) return;
            if (!cell.IsVisible) return;

            // The timer only starts when you first click any point.
            if (!_isTimerRunning)
            {
                StartTimer();
            }

            // First click: Record and highlight
            if (firstSelected == null)
            {
                firstSelected = cell;
                cell.IsSelected = true;
                return;
            }

            // Click the same one again: Deselect
            if (firstSelected == cell)
            {
                cell.IsSelected = false;
                firstSelected = null;
                return;
            }

            // Second click: Same color → Eliminate this pair
            if (firstSelected.ColorName == cell.ColorName)
            {
                firstSelected.IsVisible = false;
                cell.IsVisible = false;

                firstSelected.IsSelected = false;
                cell.IsSelected = false;

                firstSelected = null;

                RemainingPairs = Cells.Count(x => x.IsVisible) / 2;
                await CheckEnd();
            }
            else
            {
                // Different colors: Switch selection to the new point
                firstSelected.IsSelected = false;
                firstSelected = cell;
                cell.IsSelected = true;
            }
        }


        async Task CheckEnd()
        {
            bool anyVisible = Cells.Any(x => x.IsVisible);
            if (!anyVisible)
            {
                // This game has ended. First, pause the game clock and add the time to the total duration.
                StopAndSaveTimer();

                BoardActive = false;

                // Vibrate once when the game ends.
                try
                {
#if ANDROID || IOS
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));  // 0.3 senconds vibration
#endif
                }
                catch (FeatureNotSupportedException)
                {
                    // Some devices do not support vibration; this field can be left blank.
                }
                catch (Exception)
                {
                    // Other anomalies are also ignored and do not affect gameplay.
                }

#if ANDROID || WINDOWS
        await Application.Current.MainPage.DisplayAlert(
            "Great!",
            $"All dots cleared!\nTime:{SessionTimeDisplay}",
            "OK");
#endif

                GenerateBoard();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

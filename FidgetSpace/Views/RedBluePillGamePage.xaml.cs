using FidgetSpace.Models;
using FidgetSpace.Models.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;
using System.Windows.Input;
using FidgetSpace.Views;

using FidgetSpace;          // App.LoggedInUser / App.Database
using System.Threading.Tasks; //async Task

namespace FidgetSpace.Views
{

    // receive colorChoice via query parameter
    [QueryProperty(nameof(ColorChoice), "colorChoice")]
    public partial class RedBluePillGamePage : ContentPage
    {
        private int GridRows = 8;
        private int GridColumns = 5;

        private List<Ellipse> pills = new List<Ellipse>();
        private Dictionary<Ellipse, (int Row, int Col)> pillPositions = new Dictionary<Ellipse, (int Row, int Col)>();
        int totalPills = 0;
        private Random ran = new Random();
        public string? ColorChoice { get; set; }
        //public ICommand BackCommand => new Command(NavigateBack); // command to navigate back

        // set target pill position based on color choice
        private int targetRow = -1;
        private int targetColumn = -1;

        // black pill position
        private int blackPillRow = -1;
        private int blackPillColumn = -1;

        // Countdown timer vm
        readonly RedBluePillGameViewModel rbpGameTimerVm;


        private async Task NavigateBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public RedBluePillGamePage()
        {
            InitializeComponent();
            //BindingContext = this;

            rbpGameTimerVm = new RedBluePillGameViewModel();
            BindingContext = rbpGameTimerVm;

            // subscribe to time expired event so the page reacts when time is up
            rbpGameTimerVm.TimeExpired += OnTimeExpired;

            // create game board rows and columns
            for (int i = 0; i < GridRows; i++)
            {
                GameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int j = 0; j < GridColumns; j++)
            {
                GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
            GameBoard.RowSpacing = 10;
            GameBoard.ColumnSpacing = 10;

            // add ellipse to each grid cell (row+column)
            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridColumns; col++)
                {
                    var ellipse = new Ellipse
                    {
                        WidthRequest = 60,
                        HeightRequest = 60,
                        Fill = new SolidColorBrush(Colors.Green),
                    };

                    // add tap handler 
                    var tap = new TapGestureRecognizer
                    {
                        Command = new Command(() => OnPillTapped(ellipse))
                    };
                    ellipse.GestureRecognizers.Add(tap);

                    // keep track of pills and their positions
                    pills.Add(ellipse);
                    pillPositions[ellipse] = (row, col);
                    totalPills++;

                    // add ellipse to grid at specified row and column
                    GameBoard.Add(ellipse, col, row);
                }
            }
        }

        // a function to generate random colors for the pills when tapped
        private SolidColorBrush GetRandomColor()
        {
            var colorList = new List<Color>
        {
            Colors.MistyRose,
            Colors.PaleTurquoise,
            Colors.Pink,
            Colors.Orange,
            Colors.Orchid,
            Colors.Cyan,
            Colors.Lime,
            Colors.Beige,
            Colors.Turquoise,
            Colors.Gold,
            Colors.GreenYellow,
            Colors.Khaki,
            Colors.MediumPurple,
            Colors.CadetBlue,
            Colors.Teal,
        };
            int index = ran.Next(colorList.Count);
            return new SolidColorBrush(colorList[index]);
        }

        private void StartGame()
        {
            // reset the pills to default color
            foreach (var pill in pills)
            {
                pill.Fill = new SolidColorBrush(Colors.Green);
            }

            // randomly select target pill position 
            targetRow = ran.Next(0, GridRows);
            targetColumn = ran.Next(0, GridColumns);

            // randomly generate a black pill position that is not the target pill
            do
            {
                blackPillRow = ran.Next(0, GridRows);
                blackPillColumn = ran.Next(0, GridColumns);
            } while (blackPillRow == targetRow && blackPillColumn == targetColumn);

        }
        private async void OnPillTapped(Ellipse ellipse)
        {

            // Get the row and column for this ellipse
            if (!pillPositions.TryGetValue(ellipse, out var pos))
                return;

            // if run out of time
           // if ( rbpGameTimerVm.RemainingSeconds<=0)
            //{
              //  OnTimeExpired();
            //}


            // Check if the tapped pill is the target pill or black pill based on color choice
            if (pos.Row == blackPillRow && pos.Col == blackPillColumn)
            {
                // red is found, stop timer
                rbpGameTimerVm.Stop();

                await SavePillTimeToUserAsync();   // save time for this round

                // add vibration feedback
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate();
                }

                ellipse.Fill = Colors.Black;
                await DisplayAlert("The Black Pill?", "Oh… you weren’t supposed to find that. Game Over!!!", "OK");
                await Shell.Current.GoToAsync("///HomePage");
                return;
            }
            if (ColorChoice == "Red" && (pos.Row == targetRow && pos.Col == targetColumn))
            {
                // red is found, stop timer
                rbpGameTimerVm.Stop();

                await SavePillTimeToUserAsync();

                // add vibration feedback
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate();
                }

                ellipse.Fill = new SolidColorBrush(Color.FromArgb("#FF6B6B"));
                bool playAgain = await DisplayAlert($"Wow~~You found it in {rbpGameTimerVm.TimeSpentSeconds} seconds!",
                                                    "Welcome to the real world...but this is only the beginning! \n\nPlay again?",
                                                    "Yes", "No");

                if (playAgain)
                {
                    await NavigateBackAsync();   //  await
                }
                else
                {
                    await Shell.Current.GoToAsync("///HomePage");
                }
            }
            else if (ColorChoice == "Blue" && (pos.Row == targetRow && pos.Col == targetColumn))
            {
                // blue is found, stop timer
                rbpGameTimerVm.Stop();

                await SavePillTimeToUserAsync();

                if (pos.Row == blackPillRow && pos.Col == blackPillColumn)
                {
                    ellipse.Fill = Colors.Black;
                    // add vibration feedback
                    if (Vibration.Default.IsSupported)
                    {
                        Vibration.Default.Vibrate();
                    }
                    await DisplayAlert("The Black Pill?", "Oh… you weren’t supposed to find that. Game Over!!!", "OK");
                    await Shell.Current.GoToAsync("///HomePage");
                    return;
                }
                else
                {
                    ellipse.Fill = new SolidColorBrush(Color.FromArgb("#4D96FF"));
                    // add vibration feedback
                    if (Vibration.Default.IsSupported)
                    {
                        Vibration.Default.Vibrate();
                    }
                    bool playAgain = await DisplayAlert($"Wow~~Your found the blue pill in {rbpGameTimerVm.TimeSpentSeconds} seconds!", "Enjoy the calm and peace you've chosen! \n\nPlay again?", "Yes", "No");
                    if (playAgain)
                    {
                        await NavigateBackAsync();
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("///HomePage");
                    }
                }
            }
            else
            {
                ellipse.Fill = GetRandomColor();
                ellipse.GestureRecognizers.Clear(); // disable further taps on this pill
            }
        }

        // Accumulate the time spent playing the current round of Pill to the current user and update the total game time.
        private async Task SavePillTimeToUserAsync()
        {
            if (App.LoggedInUser == null)
                return;

            // If the current game time is ≤ 0, it is not recorded.
            if (rbpGameTimerVm.TimeSpentSeconds <= 0)
                return;

            // Accumulated session time (timing data in the ViewModel)
            App.LoggedInUser.TotalTimePillSeconds += rbpGameTimerVm.TimeSpentSeconds;

            // Recalculate total game time = Bubble + Dot + Pill
            App.LoggedInUser.TotalTimePlayedSeconds =
                App.LoggedInUser.TotalTimeBubbleSeconds +
                App.LoggedInUser.TotalTimeDotSeconds +
                App.LoggedInUser.TotalTimePillSeconds;

            await App.Database.Update(App.LoggedInUser);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Reset the board each time the page loads.
            StartGame();

            // Select different countdown durations based on Red/Blue selection
            int seconds;

            if (ColorChoice == "Red")
            {
                seconds = 10;
                // Target indicator dot changed to red (optional)
                EpTarget.Fill = new SolidColorBrush(Color.FromArgb("#FF6B6B"));
            }
            else
            {
                seconds = 30;
                // Default blue (optional, for greater clarity)
                EpTarget.Fill = new SolidColorBrush(Color.FromArgb("#4D96FF"));
            }

            // 只调用一次 Start
            rbpGameTimerVm.Start(seconds);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            rbpGameTimerVm.Stop();
            rbpGameTimerVm.TimeExpired -= OnTimeExpired;

        }

        async void OnTimeExpired()
        {
            await SavePillTimeToUserAsync(); // Time must be recorded even when the time limit expires.

            bool playAgain = await DisplayAlert($"Time's up! You spent {rbpGameTimerVm.TimeSpentSeconds} seconds in this game!", "\n\nPlay again?", "Yes", "No");
            if (playAgain)
            {
                await NavigateBackAsync();
            }
            else
            {
                await Shell.Current.GoToAsync("///HomePage");
            }
            return;
        }
    }
}
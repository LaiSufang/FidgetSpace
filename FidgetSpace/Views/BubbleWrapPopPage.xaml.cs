using FidgetSpace.Models;
using FidgetSpace.Models.ViewModels;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace FidgetSpace.Views
{
    public partial class BubbleWrapPopPage : ContentPage
    {
        public int bwp_Score = 0;
        public int bwp_GameTime = 0;       // seconds used in this round
        public double bwp_TotalTime = 0;
        public int currentBubbles;

        IDispatcherTimer timer;
        private int rows = 6;
        private int columns = 4;
        private int cellSize = 70;
        private int totalBubbles = 6;
        private List<Bubble> bubbles = new List<Bubble>();
        BubbleWrapPopViewModel bwp_VM = new BubbleWrapPopViewModel();
        private bool isNavigatingHome = false; // Flag to raise alert on going back 

        //  ==== (Optional) not used right now ====
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _isTimerRunning = false;

        // Game Duration (seconds) - already using bwp_GameTime
        private int sessionSeconds = 0;

        // Total cumulative seconds played across all Bubble games (read from the User database)
        private int totalTimeBubbleSeconds = 0;

        // ✅ Save bubble game time into user profile
        private async Task SaveBubbleTimeToUserAsync()
        {
            // If no user is logged in, do nothing
            if (App.LoggedInUser == null)
                return;

            // If this round took 0 seconds, don't record it
            if (bwp_GameTime <= 0)
                return;

            // ✅ Add this round's Bubble time to the user's total bubble time
            App.LoggedInUser.TotalTimeBubbleSeconds += bwp_GameTime;

            // ✅ Recalculate total play time (Bubble + Dots + Pill)
            App.LoggedInUser.TotalTimePlayedSeconds =
                App.LoggedInUser.TotalTimeBubbleSeconds +
                App.LoggedInUser.TotalTimeDotSeconds +
                App.LoggedInUser.TotalTimePillSeconds;

            // ✅ Persist changes to the database
            await App.Database.Update(App.LoggedInUser);
        }

        public BubbleWrapPopPage()
        {
            InitializeComponent();
            BindingContext = bwp_VM;

            // 1-second timer for game time
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            timer.Start();

            var boardWidth = rows * cellSize;
            var boardHeight = columns * cellSize;

            for (int i = 0; i < rows; i++)
            {
                GameBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize, GridUnitType.Absolute) });
            }

            for (int j = 0; j < columns; j++)
            {
                GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize, GridUnitType.Absolute) });
            }

            GenerateBoard();
            bwp_VM.Score = 0;
            bwp_VM.TotalTime = 0;
        }

        private void GenerateBoard()
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnBubbleClicked;

            for (int i = 0; i < totalBubbles; i++)
            {
                var bubble = new Bubble(columns, rows);
                while (bubbles.Any(b => b.x == bubble.x && b.y == bubble.y))
                {
                    bubble.regenLoc(columns, rows);
                }

                if (SpinWheel())
                {
                    bubble.Marked = true;
                }
                bubbles.Add(bubble);

                bubble.Button.GestureRecognizers.Add(tapGesture);
                bubble.Button.ZIndex = 1;
                GameBoard.Add(bubble.Button, bubble.y, bubble.x);
                Grid.SetColumn(bubble.Button, bubble.y);
                Grid.SetRow(bubble.Button, bubble.x);
            }
            currentBubbles = totalBubbles;
        }

        public void ResetBoard()
        {
            GameBoard.Children.Clear();
            bubbles.Clear();
            GenerateBoard();
        }

        private async void OnBubbleClicked(object sender, EventArgs e)
        {
            bwp_VM.Score++;
            currentBubbles--;

            // If bubble is marked, clear board and add remaining bubbles to score, then regenerate board
            if (bubbles.Any(b => b.Button == sender && b.Marked))
            {
                bwp_VM.Score += currentBubbles;
                GameBoard.Children.Clear();
                bubbles.Clear();
                GenerateBoard();
            }

            // ✅ When there are no bubbles left, the round is finished
            if (currentBubbles == 0)
            {
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate();
                }

                // Stop timer first
                timer.Stop();

                // Show the time used in this round
                bwp_VM.TotalTime = bwp_GameTime;

                // ✅ Save Bubble time from this round to the logged-in user
                await SaveBubbleTimeToUserAsync();

                bool playAgain = await DisplayAlert(
                    "Congrats!",
                    $"You popped all the bubbles in {bwp_VM.TotalTime} seconds",
                    "Play again?",
                    "Go Home");

                if (playAgain)
                {
                    // ✅ Reset round data and play again
                    bwp_GameTime = 0;          // reset seconds for new round
                    bwp_VM.TotalTime = 0;
                    currentBubbles = totalBubbles;

                    timer.Start();
                    ResetBoard();
                    bwp_VM.Score = 0;
                }
                else
                {
                    // User doesn't want to play again
                    if (bwp_VM.Score > bwp_VM.HighScore)
                    {
                        bwp_VM.HighScore = bwp_VM.Score;
                    }

                    isNavigatingHome = true;
                    await Shell.Current.GoToAsync("///HomePage");
                }
            }
        }

        private bool SpinWheel()
        {
            // Generate a random number from 1 to 10
            Random random = new Random();
            int spin = random.Next(1, 11);

            if (spin == 1)
            {
                return true; // Mark Bubble
            }
            else
            {
                return false;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // Increase game time every second
            bwp_GameTime++;
            bwp_VM.TotalTime = bwp_GameTime;
        }

        // Overrides back button press to confirm with user if they want to go home
        protected override bool OnBackButtonPressed()
        {
            if (isNavigatingHome)
            {
                // Goes back if it's been confirmed already
                return base.OnBackButtonPressed();
            }

            Dispatcher.Dispatch(async () =>
            {
                await ConfirmGoHome();
            });

            return true; // Cancel default back navigation until user confirms
        }

        // Confirms with user if they want to go home
        private async Task ConfirmGoHome()
        {
            timer.Stop();
            bool confirm = await DisplayAlert("Are you sure you want to go home?", "This game will be lost.", "Yes", "No");

            if (confirm)
            {
                isNavigatingHome = true;
                await Shell.Current.GoToAsync("///HomePage");
            }
            else
            {
                // User chose "No", resume the timer
                timer.Start();
            }
        }
    }
}

using FidgetSpace.Models;
using FidgetSpace.Models.ViewModels;
using System.Runtime.CompilerServices;

namespace FidgetSpace.Views
{
    public partial class BubbleWrapPopPage : ContentPage
    {
        public int bwp_Score = 0;
        public int bwp_GameTime = 0;
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

        //  ==== Timing-Related ====
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _isTimerRunning = false;

        // Game Duration (seconds)
        private int sessionSeconds = 0;

        // Total cumulative seconds played across all Bubble games (read from the User database)
        private int totalTimeBubbleSeconds = 0;


        public BubbleWrapPopPage()
        {
            InitializeComponent();
            BindingContext = bwp_VM;
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            timer.Start();
            // ScoreLbl.Text = $"Score: {bwp_Score}";

            var boardWidth = rows * cellSize;
            var boardHeight = columns * cellSize;
            for (int i = 0; i < rows; i++)
            {
                GameBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize, GridUnitType.Absolute) });

                //GameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            for (int j = 0; j < columns; j++)
            {
                GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize, GridUnitType.Absolute) });

                //GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
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
                //bubble.Button.Clicked += OnBubbleClicked;
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
            //bwp_Score++;
            // ScoreLbl.Text = $"Score: {bwp_Score}";
            currentBubbles--;

            // If bubble is marked, clear board and add remaining bubbles to score, then regenerate board
            if (bubbles.Any(b => b.Button == sender && b.Marked))
            {
                bwp_VM.Score += currentBubbles;
                // bwp_Score += currentBubbles;
                // ScoreLbl.Text = $"Score: {bwp_Score}";
                GameBoard.Children.Clear();
                bubbles.Clear();
                GenerateBoard();
            }

            if (currentBubbles == 0) // No more bubbles left
            {
                if (Vibration.Default.IsSupported)
                {
                    Vibration.Default.Vibrate();
                }
                timer.Stop();
                bwp_VM.TotalTime = bwp_GameTime;
                bool playAgain = await DisplayAlert("Congrats!", $"You popped all the bubbles in {bwp_VM.TotalTime} seconds", "Play again?", "Go Home");


                if (playAgain)
                {
                    timer.Start();
                    ResetBoard();
                    bwp_VM.Score = 0;
                }
                else
                { // User doesn't want to play again
                    if (bwp_VM.Score > bwp_VM.HighScore)
                    {
                        bwp_VM.HighScore = bwp_VM.Score;
                    }
                    // bwp_VM.TotalTime = bwp_TotalTime;
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
            bwp_GameTime++;
            bwp_VM.TotalTime = bwp_GameTime;
            // TimerLbl.Text = $"Total Play Time: {bwp_GameTime}s";
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
            timer.Start();
        }
    }
}
using FidgetSpace.Models;
using System.Runtime.CompilerServices;

namespace FidgetSpace.Views
{
    public partial class BubbleWrapPopPage : ContentPage
    {
        public int bwp_Score = 0;
        public int bwp_GameTime = 0;
        public double bwp_TotalTime = 0;
        IDispatcherTimer timer;
        private int rows = 6;
        private int columns = 4;
        private int cellSize = 70;
        private int totalBubbles = 6;
        private List<Bubble> bubbles = new List<Bubble>();
        private bool isNavigatingHome = false; // Flag to raise alert on going back 

        public BubbleWrapPopPage()
        {
            InitializeComponent();
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            timer.Start();
            ScoreLbl.Text = $"Score: {bwp_Score}";

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
        }

        private void GenerateBoard()
        {
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    var cellBg = new BoxView
                    {
                        // Pick a subtle color if you want to visualize cells; Transparent keeps it invisible
                        Color = Colors.Transparent,
                        InputTransparent = true, // Do not block bubble taps
                        IsEnabled = false
                    };
                    GameBoard.Add(cellBg, y, x);
                    Grid.SetColumn(cellBg, y);
                    Grid.SetRow(cellBg, x);
                }
            }
            for (int i = 0; i < totalBubbles; i++)
            {
                var bubble = new Bubble(columns, rows);
                while (bubbles.Any(b => b.x == bubble.x && b.y == bubble.y))
                {
                    bubble.regenLoc(columns, rows);
                }

                bubbles.Add(bubble);
                bubble.Button.Clicked += OnBubbleClicked;

                // Bubble.x is row, Bubble.y is column
                bubble.Button.ZIndex = 1;
                GameBoard.Add(bubble.Button, bubble.y, bubble.x);
                Grid.SetColumn(bubble.Button, bubble.y);
                Grid.SetRow(bubble.Button, bubble.x);
            }
        }

        private void ResetBoard()
        {
            bwp_Score = 0;
            ScoreLbl.Text = $"Score: {bwp_Score}";
            GameBoard.Children.Clear();
            bubbles.Clear();
            GenerateBoard();
        }

        private async void OnBubbleClicked(object sender, EventArgs e)
        {
            bwp_Score++;
            ScoreLbl.Text = $"Score: {bwp_Score}";

            if (bwp_Score >= totalBubbles) // Game finished
            {
                timer.Stop();
                bwp_TotalTime = bwp_GameTime;
                bool playAgain = await DisplayAlert("Congrats!", $"You popped all the bubbles in {bwp_TotalTime} seconds", "Play again?", "Go Home");
                
                if (playAgain)
                {
                    timer.Start();
                    ResetBoard();
                }
                else
                { // User doesn't want to play again
                    isNavigatingHome = true;
                    await Shell.Current.GoToAsync("///HomePage");
                }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            bwp_GameTime++;
            TimerLbl.Text = $"Total Play Time: {bwp_GameTime}s";
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
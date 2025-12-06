using FidgetSpace.Models;
using FidgetSpace;          // App.LoggedInUser / App.Database
using System.Diagnostics;   //Stopwatch


namespace FidgetSpace.Views
{
    public partial class BubbleWrapPopPage : ContentPage
    {
        public int score = 0;
        private readonly int rows = 6;
        private readonly int columns = 4;
        private readonly int totalBubbles = 6;
        private List<Bubble> bubbles = new List<Bubble>();

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
            ScoreLbl.Text = $"Score: {score}";

            // ==== Total duration of previous Bubbles loaded for the currently logged-in user ====
            if (App.LoggedInUser != null)
            {
                totalTimeBubbleSeconds = App.LoggedInUser.TotalTimeBubbleSeconds;
            }
            else
            {
                totalTimeBubbleSeconds = 0;
            }

            // Creates Rows
            for (int i = 0; i < rows; i++)
            {
                GameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            /* // Add Bubbles to every Grid block
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var bubble = new Bubble(columns, rows);
                    bubbles.Add(bubble);

                    GameBoard.Add(bubble.Button, j, i);
                    Grid.SetColumn(bubble.Button, j);
                    Grid.SetRow(bubble.Button, i);
                }
            }
            */
            // Add Bubbles to random Grid blocks
            for (int i = 0; i < totalBubbles; i++)
            {
                var bubble = new Bubble(columns, rows);
                bubbles.Add(bubble);
                bubble.Button.Clicked += OnBubbleClicked;
                GameBoard.Add(bubble.Button, bubble.x, bubble.y);
                Grid.SetColumn(bubble.Button, bubble.y);
                Grid.SetRow(bubble.Button, bubble.x);
            }

        } // Public BubbleWrapPopPage()

        public async void OnBubbleClicked(object sender, EventArgs e)
        {
            // The timer starts when you first click the bubble.
            if (!_isTimerRunning)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                _isTimerRunning = true;
            }

            score++;
            ScoreLbl.Text = $"Score: {score}";

            // When the score equals the total number of bubbles, the round is considered over.
            if (score >= totalBubbles)
            {
                // Stop the timer
                _isTimerRunning = false;
                _stopwatch.Stop();

                sessionSeconds = (int)_stopwatch.Elapsed.TotalSeconds;

                // Add the time taken for this round to the total Bubble time
                totalTimeBubbleSeconds += sessionSeconds;

                // If a user is logged in, write the time back to the User and update the database.
                if (App.LoggedInUser != null)
                {
                    App.LoggedInUser.TotalTimeBubbleSeconds = totalTimeBubbleSeconds;

                    // Simultaneously update the total game time (Bubble + Dot)
                    App.LoggedInUser.TotalTimePlayedSeconds =
                        App.LoggedInUser.TotalTimeBubbleSeconds +
                        App.LoggedInUser.TotalTimeDotSeconds+
                        App.LoggedInUser.TotalTimePillSeconds;

                    await App.Database.Update(App.LoggedInUser);
                }

                // Just pop up a prompt.
                await DisplayAlert(
                    "Great!",
                    $"You popped all bubbles in {sessionSeconds} seconds.",
                    "OK");

                // (Optional) Reset a game: For now, just clear the score to zero. We'll modify it later if we want to restart from scratch.
                score = 0;
                ScoreLbl.Text = $"Score: {score}";
            }
        }
    }
}
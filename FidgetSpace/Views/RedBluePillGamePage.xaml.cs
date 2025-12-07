using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using FidgetSpace.Models;
using FidgetSpace.Models.ViewModels;

namespace FidgetSpace.Views
{
    // Receives "colorChoice" from RedBluePillPage via Shell navigation
    [QueryProperty(nameof(ColorChoice), "colorChoice")]
    public partial class RedBluePillGamePage : ContentPage
    {
        private const int GridRows = 8;
        private const int GridColumns = 5;

        private readonly List<Ellipse> pills = new();
        private readonly Dictionary<Ellipse, (int Row, int Col)> pillPositions = new();
        private readonly Random ran = new();

        // Selected pill color: "Red" or "Blue"
        public string? ColorChoice { get; set; }

        // Target pill position
        private int targetRow = -1;
        private int targetColumn = -1;

        // Black pill position
        private int blackPillRow = -1;
        private int blackPillColumn = -1;

        // Countdown / timer ViewModel
        private readonly RedBluePillGameViewModel rbpGameTimerVm;

        public RedBluePillGamePage()
        {
            InitializeComponent();

            rbpGameTimerVm = new RedBluePillGameViewModel();
            BindingContext = rbpGameTimerVm;

            // Subscribe to "time expired" event
            rbpGameTimerVm.TimeExpired += OnTimeExpired;

            // Build grid layout
            for (int i = 0; i < GridRows; i++)
                GameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int j = 0; j < GridColumns; j++)
                GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            GameBoard.RowSpacing = 10;
            GameBoard.ColumnSpacing = 10;

            // Create pill ellipses
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

                    var tap = new TapGestureRecognizer
                    {
                        Command = new Command(() => OnPillTapped(ellipse))
                    };
                    ellipse.GestureRecognizers.Add(tap);

                    pills.Add(ellipse);
                    pillPositions[ellipse] = (row, col);

                    GameBoard.Add(ellipse, col, row);
                }
            }
        }

        // ===== Navigation: go back to the previous page (RedBluePillPage) =====
        private async Task NavigateBackAsync()
        {
            // ".." uses Shell navigation stack – safe back
            await Shell.Current.GoToAsync("..", true);
        }

        // ===== Random color for wrong pills =====
        private SolidColorBrush GetRandomColor()
        {
            var colorList = new List<Color>
            {
                Colors.MistyRose, Colors.PaleTurquoise, Colors.Pink, Colors.Orange,
                Colors.Orchid, Colors.Cyan, Colors.Lime, Colors.Beige, Colors.Turquoise,
                Colors.Gold, Colors.GreenYellow, Colors.Khaki, Colors.MediumPurple,
                Colors.CadetBlue, Colors.Teal,
            };

            int index = ran.Next(colorList.Count);
            return new SolidColorBrush(colorList[index]);
        }

        // ===== Initialize game board & positions =====
        private void StartGame()
        {
            foreach (var pill in pills)
                pill.Fill = new SolidColorBrush(Colors.Green);

            // Random target pill
            targetRow = ran.Next(0, GridRows);
            targetColumn = ran.Next(0, GridColumns);

            // Random black pill (different from target)
            do
            {
                blackPillRow = ran.Next(0, GridRows);
                blackPillColumn = ran.Next(0, GridColumns);
            }
            while (blackPillRow == targetRow && blackPillColumn == targetColumn);
        }

        // ===== When user taps a pill =====
        private async void OnPillTapped(Ellipse ellipse)
        {
            if (!pillPositions.TryGetValue(ellipse, out var pos))
                return;

            // --- Hit black pill => instant game over ---
            if (pos.Row == blackPillRow && pos.Col == blackPillColumn)
            {
                rbpGameTimerVm.Stop();
                await SavePillTimeToUserAsync();   // Save session time

                if (Vibration.Default.IsSupported)
                    Vibration.Default.Vibrate();

                ellipse.Fill = Colors.Black;
                await DisplayAlert(
                    "The Black Pill?",
                    "Oh… you weren’t supposed to find that. Game Over!!!",
                    "OK");

                // Go back to Home (root)
                await Shell.Current.GoToAsync("//HomePage");
                return;
            }

            // --- RED pill game ---
            if (ColorChoice == "Red" &&
                pos.Row == targetRow && pos.Col == targetColumn)
            {
                rbpGameTimerVm.Stop();
                await SavePillTimeToUserAsync();

                if (Vibration.Default.IsSupported)
                    Vibration.Default.Vibrate();

                ellipse.Fill = new SolidColorBrush(Color.FromArgb("#FF6B6B"));

                bool playAgain = await DisplayAlert(
                    $"Wow~~ You found it in {rbpGameTimerVm.TimeSpentSeconds} seconds!",
                    "Welcome to the real world... but this is only the beginning!\n\nPlay again?",
                    "Yes", "No");

                if (playAgain)
                    await NavigateBackAsync();       // Back to pill selection page
                else
                    await Shell.Current.GoToAsync("//HomePage");

                return;
            }

            // --- BLUE pill game ---
            if (ColorChoice == "Blue" &&
                pos.Row == targetRow && pos.Col == targetColumn)
            {
                rbpGameTimerVm.Stop();
                await SavePillTimeToUserAsync();

                if (pos.Row == blackPillRow && pos.Col == blackPillColumn)
                {
                    // Super rare case: target == black pill (we avoided it, but keep guard)
                    ellipse.Fill = Colors.Black;
                    if (Vibration.Default.IsSupported)
                        Vibration.Default.Vibrate();

                    await DisplayAlert(
                        "The Black Pill?",
                        "Oh… you weren’t supposed to find that. Game Over!!!",
                        "OK");
                    await Shell.Current.GoToAsync("//HomePage");
                    return;
                }
                else
                {
                    ellipse.Fill = new SolidColorBrush(Color.FromArgb("#4D96FF"));

                    if (Vibration.Default.IsSupported)
                        Vibration.Default.Vibrate();

                    bool playAgain = await DisplayAlert(
                        $"Wow~~ You found the blue pill in {rbpGameTimerVm.TimeSpentSeconds} seconds!",
                        "Enjoy the calm and peace you've chosen!\n\nPlay again?",
                        "Yes", "No");

                    if (playAgain)
                        await NavigateBackAsync();
                    else
                        await Shell.Current.GoToAsync("//HomePage");

                    return;
                }
            }

            // --- Wrong pill: random color & disable further taps ---
            ellipse.Fill = GetRandomColor();
            ellipse.GestureRecognizers.Clear();
        }

        // ===== Save pill game info into the current user =====
        private async Task SavePillTimeToUserAsync()
        {
            if (App.LoggedInUser == null)
                return;

            // Ignore empty sessions
            if (rbpGameTimerVm.TimeSpentSeconds <= 0)
                return;

            // Accumulate pill game time
            App.LoggedInUser.TotalTimePillSeconds += rbpGameTimerVm.TimeSpentSeconds;

            // Save last game info
            App.LoggedInUser.LastGameDuration = rbpGameTimerVm.TimeSpentSeconds;
            App.LoggedInUser.LastPillChoice = ColorChoice ?? string.Empty;
            App.LoggedInUser.LastGamePlayed = DateTime.Now;

            // Recalculate total game time
            App.LoggedInUser.TotalTimePlayedSeconds =
                App.LoggedInUser.TotalTimeBubbleSeconds +
                App.LoggedInUser.TotalTimeDotSeconds +
                App.LoggedInUser.TotalTimePillSeconds;

            await App.Database.Update(App.LoggedInUser);
        }

        // ===== Page lifecycle =====
        protected override void OnAppearing()
        {
            base.OnAppearing();

            StartGame();

            int seconds;
            if (ColorChoice == "Red")
            {
                seconds = 10;
                EpTarget.Fill = new SolidColorBrush(Color.FromArgb("#FF6B6B"));
            }
            else
            {
                seconds = 30;
                EpTarget.Fill = new SolidColorBrush(Color.FromArgb("#4D96FF"));
            }

            rbpGameTimerVm.Start(seconds);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            rbpGameTimerVm.Stop();
            rbpGameTimerVm.TimeExpired -= OnTimeExpired;
        }

        // ===== When countdown reaches zero =====
        private async void OnTimeExpired()
        {
            await SavePillTimeToUserAsync();

            bool playAgain = await DisplayAlert(
                $"Time's up! You spent {rbpGameTimerVm.TimeSpentSeconds} seconds!",
                "Play again?",
                "Yes", "No");

            if (playAgain)
                await NavigateBackAsync();
            else
                await Shell.Current.GoToAsync("//HomePage");
        }

        // ===== Top Back button (Clicked="BackButton_Clicked" in XAML) =====
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            rbpGameTimerVm?.Stop();
            await SavePillTimeToUserAsync();
            await NavigateBackAsync();
        }
    }
}

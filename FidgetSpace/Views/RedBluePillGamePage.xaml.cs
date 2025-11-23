using FidgetSpace.Models;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

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
        public ICommand BackCommand => new Command(NavigateBack); // command to navigate back

        // set target pill position based on color choice
        private int targetRow = -1;
        private int targetColumn = -1;

        // black pill position
        private int blackPillRow = -1;
        private int blackPillColumn = -1;

        private async void NavigateBack()
        {
            await Shell.Current.GoToAsync(nameof(RedBluePillPage));
        }

        public RedBluePillGamePage()
        {
            InitializeComponent();
            BindingContext = this;

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
            Colors.Purple,
            Colors.Pink,
            Colors.Orange,
            Colors.Orchid,
            Colors.Cyan,
            Colors.Lime,
            Colors.Magenta,
            Colors.Turquoise,
            Colors.Gold,
            Colors.GreenYellow,
            Colors.Lavender,
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

            // Check if the tapped pill is the target pill or black pill based on color choice
            if (ColorChoice == "Red" && (pos.Row == targetRow && pos.Col == targetColumn))
            {
                if (pos.Row == blackPillRow && pos.Col == blackPillColumn)
                {
                    ellipse.Fill = Colors.Black;
                    await DisplayAlert("The Black Pill?", "Oh… you weren’t supposed to find that. Game Over!!!", "OK");
                    await Shell.Current.GoToAsync("///HomePage");
                    return;
                }
                else 
                { 
                    ellipse.Fill = Colors.Red;
                    bool playAgain = await DisplayAlert("Your found it!", "Welcome to the real world...but this is only the beginning! Play again?", "Yes", "No");
                    if (playAgain)
                    {
                        NavigateBack();
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("///HomePage");
                    }
                }

            }
            else if (ColorChoice == "Blue" && (pos.Row == targetRow && pos.Col == targetColumn))
            {
                if (pos.Row == blackPillRow && pos.Col == blackPillColumn)
                {
                    ellipse.Fill = Colors.Black;
                    await DisplayAlert("The Black Pill?", "Oh… you weren’t supposed to find that. Game Over!!!", "OK");
                    await Shell.Current.GoToAsync("///HomePage");
                    return;
                }
                else
                {
                    ellipse.Fill = Colors.Blue;
                    bool playAgain = await DisplayAlert("Blue Pill found!", "Enjoy the calm and peace you've chosen! Play again?", "Yes", "No");
                    if (playAgain)
                    {
                        NavigateBack();
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


            // TODO: If the black pill is found, the game ends: The Black Pill? Oh… you weren’t supposed to find that. Game ends here.

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //LbColor.Text += ColorChoice;
            EpTarget.Fill = ColorChoice == "Red" ? Colors.Red : Colors.Blue;
            ;

            StartGame();

            // TODO: add a timer for the game session
        }


    }
}
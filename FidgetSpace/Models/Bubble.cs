using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Models
{
    public class Bubble
    {
        public Image Button { get; set; }
        public bool Marked { get; set; }
        public int x { get; set; }
        public int y { get; set; }

<<<<<<< HEAD
        // Reference: https://www.telerik.com/blogs/using-csharp-markup-create-graphical-interfaces-net-maui
        Button = new Button
        {
            WidthRequest = 60,         
            HeightRequest = 60,
            CornerRadius = 30,         
            BackgroundColor = Colors.SeaGreen,
            ImageSource = "whitemiddle.png",        
            Margin = 6
        };
        Button.Clicked += OnBubbleClicked;
    }

    public void OnBubbleClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        button.IsVisible = false;
=======
        private static readonly Random random = new Random();

        public Bubble(int col, int row)
        {
            Marked = false;
            // Sets X and Y coordinates
            x = (int)(random.NextDouble() * row);
            y = (int)(random.NextDouble() * col);

            /*
            // Reference: https://www.telerik.com/blogs/using-csharp-markup-create-graphical-interfaces-net-maui?
            Button = new Button
            {
                WidthRequest = 60,
                HeightRequest = 60,
                CornerRadius = 30,
                BackgroundColor = Colors.White,
                ImageSource = "tealbubble.png",
                Margin=5, 
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Colors.DarkSeaGreen),
                    Offset = new Point(3, 3),
                    Opacity = 0.3f,
                    Radius = 5
                }
            };
            Button.Clicked += OnBubbleClicked;*/
            Button = new Image
            {
                Source = "tealbubble.png",
                WidthRequest = 60,
                HeightRequest = 60,
                Margin = 5
            };
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnBubbleClicked;
            Button.GestureRecognizers.Add(tapGesture);
        }

        private void OnBubbleClicked(object sender, EventArgs e)
        {
            Button.Opacity = 0;
            Button.IsEnabled = false;
        }

        public void regenLoc(int col, int row)
        {
            x = (int)(random.NextDouble() * row);
            y = (int)(random.NextDouble() * col);
        }
>>>>>>> main
    }
}
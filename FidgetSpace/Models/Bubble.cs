using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Models;
class Bubble
{
    public Button Button { get; set; }
    public bool Marked { get; set; }
    public int x { get; set; }
    public int y { get; set; }

    private static readonly Random rng = new();

    public Bubble(int col, int row)
    {
        Marked = false;
        // Sets X and Y coordinates
        x = rng.Next(0, row);
        y = rng.Next(0, col);

        // Reference: https://www.telerik.com/blogs/using-csharp-markup-create-graphical-interfaces-net-maui?
        Button = new Button
        {
            WidthRequest = 60,         
            HeightRequest = 60,
            CornerRadius = 30,         
            BackgroundColor = Colors.White,
            ImageSource="tealbubble.png",
            Text = string.Empty,        
            Margin = 6,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.DarkSeaGreen),
                Offset = new Point(3, 3),
                Opacity = 0.3f,
                Radius = 5
            }
        };
        Button.Clicked += OnBubbleClicked;
    }

     public void OnBubbleClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        button.IsVisible = false;
    }
}

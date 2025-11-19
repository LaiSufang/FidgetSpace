using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Models;

class BubbleUnit
{
    public Ellipse Button { get; set; }
    public bool Marked { get; set; }
    public int x { get; set; }
    public int y { get; set; }

    private static readonly Random rng = new();

    public BubbleUnit(int col, int row)
    {
        Marked = false;
        // Sets X and Y coordinates
        x = rng.Next(0, row);
        y = rng.Next(0, col);

        Button = new Ellipse
        {
            WidthRequest = 60,
            HeightRequest = 60,
            Background = new RadialGradientBrush
            {
                GradientStops =
                {
                new GradientStop { Offset = 0.1f, Color = Colors.Blue },
                new GradientStop { Offset = 1.0f, Color = Colors.LightBlue }
                }
            }
        };
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnBubbleClicked;
        Button.GestureRecognizers.Add(tapGesture);
    }

    public void OnBubbleClicked(object sender, EventArgs e)
    {
        if (sender is VisualElement element)
        {
            element.IsVisible = false;
        }
    }
}

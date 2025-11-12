using FidgetSpace.Models;

namespace FidgetSpace;

public partial class BubbleWrapPopPage : ContentPage
{
    public int score = 0;
    private readonly int rows = 6;
    private readonly int columns = 4;
    private readonly int totalBubbles = 6;
    private List<Bubble> bubbles = new List<Bubble>();

    public BubbleWrapPopPage()
    {
        InitializeComponent();
        ScoreLbl.Text = $"Score: {score}";
        // Creates Rows
        for (int i = 0; i < rows; i++)
        {
            GameBoard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Creates Columns
        for (int j = 0; j < columns; j++)
        {
            GameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
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

    public void OnBubbleClicked(object sender, EventArgs e)
    {
        score++;
        ScoreLbl.Text = $"Score: {score}";
    }
}
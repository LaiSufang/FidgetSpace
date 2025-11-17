using System.Threading.Tasks;

namespace FidgetSpace;

public partial class RedBluePillPage : ContentPage
{
	public RedBluePillPage()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        EpRed.HeightRequest = 120;
        EpRed.WidthRequest = 120;

        PillChoice.Text = $"You chose Red Pill! \nStart the game and find the red pill within 10 seconds!";
        PillChoice.TextColor = Microsoft.Maui.Graphics.Colors.Red;

        EpBlue.HeightRequest = 100;
        EpBlue.WidthRequest = 100;
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        EpBlue.HeightRequest = 120;
        EpBlue.WidthRequest = 120;

        PillChoice.Text = $"You chose Blue Pill! \nStart the game and find the blue pill within 10 seconds!";
        PillChoice.TextColor = Microsoft.Maui.Graphics.Colors.Blue;
        EpRed.HeightRequest = 100;
        EpRed.WidthRequest = 100;

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RedBluePillGamePage));
    }
}
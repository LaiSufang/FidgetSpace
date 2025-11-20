using System.Threading.Tasks;
using System.Windows.Input;

namespace FidgetSpace;

public partial class RedBluePillPage : ContentPage
{
    //public ICommand BackCommand => new Command(NavigateBack);

    //private async void NavigateBack()
    //{
    //    await Shell.Current.GoToAsync("///HomePage");
    //}

    private string colorChoice = string.Empty;
    public RedBluePillPage()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        EpRed.HeightRequest = 120;
        EpRed.WidthRequest = 120;

        FrPillChoice.IsVisible = true;
        PillChoice.Text = $"Red Pill! Be ready to embrace a difficult truth! Start game and find the red pill within 10 seconds!";
        //PillChoice.TextColor = Microsoft.Maui.Graphics.Colors.Red;

        EpBlue.HeightRequest = 100;
        EpBlue.WidthRequest = 100;
        
        colorChoice = "Red";
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        EpBlue.HeightRequest = 120;
        EpBlue.WidthRequest = 120;

        FrPillChoice.IsVisible = true;
        PillChoice.Text = $"Blue Pill! It's good to stay in blissful ignorance! Start game and find the blue pill within 10 seconds!";
        //PillChoice.TextColor = Microsoft.Maui.Graphics.Colors.Blue;
        EpRed.HeightRequest = 100;
        EpRed.WidthRequest = 100;

        colorChoice = "Blue";

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        
        if (!string.IsNullOrEmpty(colorChoice))
        {
            var myData = new Dictionary<string, object>
            {
                {"colorChoice", colorChoice}
            };
            // Pass the colorChoice as a query parameter using a dictionary
            await Shell.Current.GoToAsync(nameof(RedBluePillGamePage), myData);
        }
        else 
        {
            await DisplayAlert("No Pill Selected", "Please select a pill color before starting the game.", "OK");
            return;
        }
    }
}
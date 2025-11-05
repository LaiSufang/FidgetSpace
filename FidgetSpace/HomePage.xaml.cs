namespace FidgetSpace;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private async void BtnBubbleWrapPlay_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BubbleWrapPopPage));
    }

    private async void BtnConnectingDotsPlay_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ConnectingDotsPage));
    }

    private async void BtnRedBluePillPlay_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RedBluePillPage));
    }
}
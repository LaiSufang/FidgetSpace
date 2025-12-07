using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FidgetSpace.Views
{
    public partial class RedBluePillPage : ContentPage
    {
        // Store the current pill color choice ("Red" or "Blue")
        private string colorChoice = string.Empty;

        public RedBluePillPage()
        {
            InitializeComponent();
        }

        // (Optional) Back helper – currently not used
        private async Task NavigateBackAsync()
        {
            // Go back one page using Shell navigation stack
            await Shell.Current.GoToAsync("..", true);
        }

        // User taps the RED pill
        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            // Enlarge red pill
            EpRed.HeightRequest = 120;
            EpRed.WidthRequest = 120;

            // Shrink blue pill
            EpBlue.HeightRequest = 100;
            EpBlue.WidthRequest = 100;

            // Show description
            FrPillChoice.IsVisible = true;
            PillChoice.Text =
                "Red Pill selected! Be ready for a difficult truth! " +
                "You have 10 seconds to find it...Go!";

            colorChoice = "Red";
        }

        // User taps the BLUE pill
        private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
        {
            // Enlarge blue pill
            EpBlue.HeightRequest = 120;
            EpBlue.WidthRequest = 120;

            // Shrink red pill
            EpRed.HeightRequest = 100;
            EpRed.WidthRequest = 100;

            // Show description
            FrPillChoice.IsVisible = true;
            PillChoice.Text =
                "Blue Pill...Entering bliss mode! " +
                "You've got 30 seconds to find the blue pill!";

            colorChoice = "Blue";
        }

        // Start game button
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(colorChoice))
            {
                await DisplayAlert(
                    "No Pill Selected",
                    "Please select a pill color before starting the game.",
                    "OK");
                return;
            }

            // Pass the selected pill color to the game page
            var myData = new Dictionary<string, object>
            {
                { "colorChoice", colorChoice }   // key must match QueryProperty on game page
            };

            await Shell.Current.GoToAsync(nameof(RedBluePillGamePage), myData);
        }

        // Back button on this selection page (if you have one in XAML)
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // For example: go back to HomePage
            await Shell.Current.GoToAsync("//HomePage");
        }
    }
}

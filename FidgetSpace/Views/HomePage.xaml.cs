using FidgetSpace.Models.ViewModels;
using FidgetSpace.Views;
using System;
using FidgetSpace.Models;
using FidgetSpace.Models;
namespace FidgetSpace.Views

{


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
            await Shell.Current.GoToAsync(nameof(ConnectDotsPage));
        }

        private async void BtnRedBluePillPlay_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RedBluePillPage));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(UserProfilePage));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(
                new SettingsPage(
                    new SettingsViewModel(((App)Application.Current).MusicService, App.Database)
                )
            );
        }

        private async void Button_Logout(object sender, EventArgs e)
        {
            
            await Shell.Current.GoToAsync(nameof(Signin));
        }

       
    }
}
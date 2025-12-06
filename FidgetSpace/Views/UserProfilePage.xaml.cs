using FidgetSpace;
using FidgetSpace.Models;
using FidgetSpace.Models.ViewModels;

namespace FidgetSpace.Views
{
    public partial class UserProfilePage : ContentPage
    {
        public UserProfilePage()
        {
            InitializeComponent();

            var userlogininfo = App.LoggedInUser; // store this when user logs in
            BindingContext = new UserProfileViewModel(userlogininfo);
        }

        private async void OnChangePasswordTapped(object sender, EventArgs e)
        {
            string newPass = await DisplayPromptAsync(
                "Change Password",
                "Enter new password:",
                "Save",
                "Cancel");

            if (!string.IsNullOrEmpty(newPass))
            {
                var vm = (UserProfileViewModel)BindingContext;
                vm.User.Password = newPass;

                await App.Database.Update(vm.User);

                await DisplayAlert("Success", "Password updated!", "OK");
            }
        }
        public async Task LoadStats()
        {
            var user = App.LoggedInUser;

            if (user != null)
            {
                LastGameDuration = user.LastGameDuration;
                LastPillChoice = user.LastPillChoice;
                LastGamePlayed = user.LastGamePlayed;
            }
        }

    }
}

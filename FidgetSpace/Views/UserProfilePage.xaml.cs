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

            // Pass logged-in user into ViewModel
            var userLoginInfo = App.LoggedInUser;

            BindingContext = new UserProfileViewModel(userLoginInfo);
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

                // Update password in memory
                vm.User.Password = newPass;

                // Save to database
                await App.Database.Update(vm.User);

                await DisplayAlert("Success", "Password updated!", "OK");
            }
        }
    }
}

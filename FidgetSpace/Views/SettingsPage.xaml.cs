using FidgetSpace.Models.ViewModels;
using FidgetSpace.Services;

namespace FidgetSpace.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;  // Use the SAME ViewModel always
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
                var vm = (SettingsViewModel)BindingContext;

                vm.CurrentUser.Password = newPass;
                await App.Database.Update(vm.CurrentUser);

                await DisplayAlert("Success", "Password updated!", "OK");
            }
        }
    }
}

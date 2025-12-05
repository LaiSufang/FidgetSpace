using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FidgetSpace.Services;
using FidgetSpace.Views;
using System.Linq;
using System.Threading.Tasks;

namespace FidgetSpace.Models.ViewModels
{
    public partial class SignupViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty] string username;
        [ObservableProperty] string password;
        [ObservableProperty] string email;
        [ObservableProperty] string phone;
        public SignupViewModel()
        {
            _db = new DatabaseService();
        }

        [RelayCommand]
        public async Task Signup()
        {
            var users = await _db.GetUsers();

            // Check if username already exists
            if (users.Any(u => u.Username == Username))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Username already taken.",
                    "OK");
                return;
            }

            // Create new user
            var newUser = new User
            {
                Username = Username,
                Password = Password,
                Email = Email,
                Phone = Phone

            };

            await _db.Create(newUser);

            await Application.Current.MainPage.DisplayAlert(
                "Success",
                "Account created!",
                "OK");

            // Navigate manually since no Shell
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}

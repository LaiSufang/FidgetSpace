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
        [ObservableProperty] string confirmPassword;

        public SignupViewModel()
        {
            _db = new DatabaseService();
        }

        

        [RelayCommand]
        public async Task Signup()
        {
            var users = await _db.GetUsers();

            // Username must not repeat
            if (string.IsNullOrWhiteSpace(Username))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Username cannot be empty.",
                    "OK");
                return;
            }

            // Username cannot start with a number
            if (char.IsDigit(Username[0]))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Username cannot start with a number.",
                    "OK");
                return;
            }

            // Username only letters, digits, underscores
            if (!Username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Username can only contain letters, numbers, and underscores.",
                    "OK");
                return;
            }

            // Password: strong security requirements
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Password must be at least 8 characters long.",
                    "OK");
                return;
            }

            // Must contain uppercase
            if (!Password.Any(char.IsUpper))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Password must contain at least one uppercase letter.",
                    "OK");
                return;
            }

            // Must contain lowercase
            if (!Password.Any(char.IsLower))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Password must contain at least one lowercase letter.",
                    "OK");
                return;
            }

            // Must contain digit
            if (!Password.Any(char.IsDigit))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Password must contain at least one number.",
                    "OK");
                return;
            }

            // Must contain special character
            if (!Password.Any(c => "!@#$%^&*()_+-={}[]|:;<>,.?/~`".Contains(c)))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Password must contain at least one special character.",
                    "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(ConfirmPassword) ||
                 Password?.Trim() != ConfirmPassword?.Trim())
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Passwords do not match. Please try again.",
                    "OK");
                return;
            }


            // Email validation using regex
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(Email) ||
            !System.Text.RegularExpressions.Regex.IsMatch(Email, emailRegex))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Please enter a valid email address with a valid domain.",
                    "OK");
                return;
            }

            // Phone validation: only digits
            if (string.IsNullOrWhiteSpace(Phone) || !Phone.All(char.IsDigit))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Phone number must contain only numbers.",
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

            // Navigate back
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
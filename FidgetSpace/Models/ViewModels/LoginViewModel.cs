using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FidgetSpace.Services;
using FidgetSpace.Views;
using Plugin.Maui.Audio;
using System.Diagnostics;

namespace FidgetSpace.Models.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty] string username;
        [ObservableProperty] string password;

        public LoginViewModel()
        {
            _db = new DatabaseService();

        }

        [RelayCommand]
        public async Task Login()
        {
            var users = await _db.GetUsers();

            var user = users.FirstOrDefault(u =>
                u.Username == Username && u.Password == Password);

            if (user == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Login Failed",
                    "Invalid username or password.",
                    "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert(
                "Success",
                $"Welcome {user.Username}!",
                "OK");

            // Navigate to home or dashboard
             await Shell.Current.GoToAsync("//HomePage");

            App.LoggedInUser = user;
            var app = Application.Current as App;
            if (app?.MusicService != null)
            {
              
                if (user.MusicEnabled)
                    await app.MusicService.Play();
                else
                    app.MusicService.Pause();
            }

            await Shell.Current.GoToAsync("//HomePage");
        }



        [RelayCommand]
        public async Task GoToSignup()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SignupPage());
        }
    }
}

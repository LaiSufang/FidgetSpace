using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using FidgetSpace.Services;


namespace FidgetSpace.Models.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly MusicService _musicService;
        private readonly DatabaseService _db;

        public User CurrentUser => App.LoggedInUser;

        [ObservableProperty] private bool musicEnabled;
        [ObservableProperty] private double musicVolume;

        public string VolumeLabel => $"Volume: {(int)(MusicVolume * 100)}%";

        public SettingsViewModel(MusicService musicService, DatabaseService db)
        {
            _musicService = musicService;
            _db = db;

            if (CurrentUser == null)
                return;

            MusicEnabled = CurrentUser.MusicEnabled;
            MusicVolume = CurrentUser.MusicVolume;

            if (MusicEnabled)
                _musicService.Play();

            _musicService.SetVolume(MusicVolume);
        }

        [RelayCommand]
        public async Task Save()
        {
            CurrentUser.MusicEnabled = MusicEnabled;
            CurrentUser.MusicVolume = MusicVolume;

            await _db.Update(CurrentUser);

            if (MusicEnabled)
                await _musicService.Play();
            else
                _musicService.Pause();

            _musicService.SetVolume(MusicVolume);

            await Application.Current.MainPage.DisplayAlert(
                "Saved",
                "Music settings saved!",
                "OK");
        }
    }
}

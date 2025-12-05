using FidgetSpace.Models.ViewModels;
using FidgetSpace.Services;
using Plugin.Maui.Audio;

namespace FidgetSpace.Views
{

	public partial class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			InitializeComponent();
            // Ensure the page has a ViewModel instance so bindings (SaveCommand, MusicEnabled, etc.) work.
            // Use simple construction with the same services your app uses.
            var app = Application.Current as App;
            var musicService = app?.MusicService ?? new MusicService(AudioManager.Current);
            BindingContext = new SettingsViewModel(musicService, App.Database);
        }
    }
}

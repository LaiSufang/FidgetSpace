using FidgetSpace.Models;
using FidgetSpace.Services;

namespace FidgetSpace
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }

        public static User LoggedInUser { get; set; }

        public MusicService MusicService { get; }

        public App(MusicService musicService)
        {
            InitializeComponent();

            Database = new DatabaseService();

            MusicService = musicService;

            // Load previously logged-in user if any (keeps existing behavior)
            LoggedInUser = LoadUserFromPreferencesOrNull();

            if (LoggedInUser != null && LoggedInUser.MusicEnabled)
            {
                MusicService.SetVolume(LoggedInUser.MusicVolume);
                _ = MusicService.Play();
            }

            MainPage = new AppShell();
        }

        private User LoadUserFromPreferencesOrNull()
        {
            
            return null;
        }
    }
}

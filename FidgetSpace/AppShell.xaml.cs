

namespace FidgetSpace
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(UserProfilePage), typeof(UserProfilePage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(BubbleWrapPopPage), typeof(BubbleWrapPopPage));
            Routing.RegisterRoute(nameof(ConnectingDotsPage), typeof(ConnectingDotsPage));
            Routing.RegisterRoute(nameof(RedBluePillPage), typeof(RedBluePillPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }


    }
}

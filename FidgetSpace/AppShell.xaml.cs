using FidgetSpace.Views;
using Microsoft.Maui.Controls;

namespace FidgetSpace
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
            Routing.RegisterRoute(nameof(ConnectDotsPage), typeof(ConnectDotsPage));
            Routing.RegisterRoute(nameof(UserProfilePage), typeof(UserProfilePage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(BubbleWrapPopPage), typeof(BubbleWrapPopPage));
            Routing.RegisterRoute(nameof(RedBluePillPage), typeof(RedBluePillPage));
            Routing.RegisterRoute(nameof(RedBluePillGamePage), typeof(RedBluePillGamePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }


    }
}

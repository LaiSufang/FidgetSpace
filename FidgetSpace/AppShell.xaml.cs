using FidgetSpace.Views;
using Microsoft.Maui.Controls;

namespace FidgetSpace
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // for signin/signup
            Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
            Routing.RegisterRoute(nameof(Signin), typeof(Signin));

            // Games pages
            Routing.RegisterRoute(nameof(ConnectDotsPage), typeof(ConnectDotsPage));
            Routing.RegisterRoute(nameof(BubbleWrapPopPage), typeof(BubbleWrapPopPage));
            Routing.RegisterRoute(nameof(RedBluePillPage), typeof(RedBluePillPage));
            Routing.RegisterRoute(nameof(RedBluePillGamePage), typeof(RedBluePillGamePage));

            // profile page
            Routing.RegisterRoute(nameof(UserProfilePage), typeof(UserProfilePage));

            // setting page
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}

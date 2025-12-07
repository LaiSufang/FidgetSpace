using FidgetSpace.Views;
using Microsoft.Maui.Controls;

namespace FidgetSpace
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // 账号相关
            Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
            Routing.RegisterRoute(nameof(Signin), typeof(Signin));

            // 游戏页面
            Routing.RegisterRoute(nameof(ConnectDotsPage), typeof(ConnectDotsPage));
            Routing.RegisterRoute(nameof(BubbleWrapPopPage), typeof(BubbleWrapPopPage));
            Routing.RegisterRoute(nameof(RedBluePillPage), typeof(RedBluePillPage));
            Routing.RegisterRoute(nameof(RedBluePillGamePage), typeof(RedBluePillGamePage));

            // 🔹 这行是关键：给 UserProfilePage 注册路由
            Routing.RegisterRoute(nameof(UserProfilePage), typeof(UserProfilePage));

            // 设置页面
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}

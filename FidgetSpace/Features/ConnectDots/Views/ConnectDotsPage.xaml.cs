using Microsoft.Maui.Controls;

namespace FidgetSpace.Features.ConnectDots.Views
{
    /// <summary>
    /// 这个页面只负责初始化，不写业务逻辑
    /// 真正的逻辑都在 ConnectDotsViewModel 里
    /// </summary>
    public partial class ConnectDotsPage : ContentPage
    {
        public ConnectDotsPage()
        {
            InitializeComponent();
            // BindingContext 已经在 XAML 里设置，这里不用再写
        }
    }
}

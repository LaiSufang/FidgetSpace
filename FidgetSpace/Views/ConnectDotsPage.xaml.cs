using Microsoft.Maui.Controls;
using FidgetSpace.Models.ViewModels;   // ✅ 你的 ViewModel 命名空间

namespace FidgetSpace.Views
{
    /// <summary>
    /// 这个页面只负责初始化和生命周期钩子
    /// 真正的逻辑都在 ConnectDotsViewModel 里
    /// </summary>
    public partial class ConnectDotsPage : ContentPage
    {
        private ConnectDotsViewModel _viewModel;

        public ConnectDotsPage()
        {
            InitializeComponent();

            // 从 XAML 绑定中拿到 ViewModel 实例
            _viewModel = BindingContext as ConnectDotsViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // ⭐ 不在这里启动计时器，改为第一次点击时开始
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // 离开页面时保存本局时间并停止计时
            _viewModel?.StopAndSaveTimer();
        }
    }
}

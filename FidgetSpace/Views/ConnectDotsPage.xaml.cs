using Microsoft.Maui.Controls;
using FidgetSpace.Models.ViewModels;

namespace FidgetSpace.Views
{
    /// <summary>
    /// Code-behind for the Connect the Dots page.
    /// The game logic is handled in ConnectDotsViewModel.
    /// </summary>
    public partial class ConnectDotsPage : ContentPage
    {
        private readonly ConnectDotsViewModel _viewModel;

        public ConnectDotsPage()
        {
            InitializeComponent();

            // Create the ViewModel and assign it to BindingContext
            _viewModel = new ConnectDotsViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // We start the timer on the first dot tap, not here.
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // When leaving the page, stop the timer and save the time.
            _viewModel.StopAndSaveTimer();
        }
    }
}
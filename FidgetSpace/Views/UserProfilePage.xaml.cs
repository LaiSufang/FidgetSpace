using FidgetSpace;
using FidgetSpace.Models;
using FidgetSpace.Models.ViewModels;

namespace FidgetSpace.Views
{
    public partial class UserProfilePage : ContentPage
    {
        public UserProfilePage()
        {
            InitializeComponent();

            // Pass logged-in user into ViewModel
            var userLoginInfo = App.LoggedInUser;

            BindingContext = new UserProfileViewModel(userLoginInfo);
        }

       
    }
}

namespace FidgetSpace;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(UserName.Text)
            && !string.IsNullOrEmpty(Email.Text)
            && !string.IsNullOrEmpty(Password.Text)
            && (Password.Text == ConfirmPassword.Text))
        {
     

            // Pass data using a dictionary
            var myData = new Dictionary<string, object>
                {
                    {"username", UserName.Text},
                    {"email", Email.Text},
                    {"password", Password.Text}
                };

            await Shell.Current.GoToAsync(nameof(UserProfilePage), myData);
            // same as passing the data directly as the following:
            //Shell.Current.GoToAsync($"{nameof(ProfilePage)}?username={UserName.Text}&email={Email.Text}&password{Password.Text}");

            // reset the entries
            UserName.Text = "";
            Email.Text = "";
            Password.Text = "";
            ConfirmPassword.Text = "";
        }
        else if (string.IsNullOrEmpty(UserName.Text))
        {
            await DisplayAlert("Unable to sign up!", "Please enter your user name", "OK");
        }
        else if (string.IsNullOrEmpty(Email.Text))
        {
            await DisplayAlert("Unable to sign up!", "Please enter your email", "OK");
        }
        else if (string.IsNullOrEmpty(Password.Text))
        {
            await DisplayAlert("Unable to sign up!", "Please enter your password", "OK");
        }
        else if (string.IsNullOrEmpty(ConfirmPassword.Text))
        {
            await DisplayAlert("Unable to sign up!", "Please confirm your password", "OK");
        }
        else if (Password.Text != ConfirmPassword.Text)
        {
            await DisplayAlert("Unable to sign up!", "Please make sure your passwords match", "OK");

            // reset the entries
            ConfirmPassword.Text = "";
        }
        
    }

    private async void BtnSkip_Clicked(object sender, EventArgs e)
    {

        await Shell.Current.GoToAsync(nameof(HomePage));
    }
}
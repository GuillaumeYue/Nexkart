namespace NexKart.Pages.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnGoToSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Validation", "Please enter email and password.", "OK");
            return;
        }

        try
        {
            await App.Auth.SignInAsync(EmailEntry.Text.Trim(), PasswordEntry.Text);
            await Navigation.PushAsync(new MainPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Login Failed", ex.Message, "OK");
        }
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        if (!App.GoogleAuth.IsSupportedOnCurrentPlatform())
        {
            await DisplayAlert("Google Login", "Google login is not supported on Windows.", "OK");
            return;
        }

        try
        {
            bool ok = await App.GoogleAuth.LoginWithGoogleAsync();
            if (!ok)
            {
                await DisplayAlert("Google Login", "Google login failed.", "OK");
                return;
            }

            await Navigation.PushAsync(new MainPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Google Login", ex.Message, "OK");
        }
    }
}

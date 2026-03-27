namespace NexKart.Pages.Auth;
using NexKart.Services;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseService _firebaseService = new();
    private readonly GoogleAuthService _googleAuthService = new();

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
            _ = await _firebaseService.GetProducts();
            await Navigation.PushAsync(new MainPage());
        }
        catch
        {
            await DisplayAlert("Firebase", "Cannot connect to Realtime Database.", "OK");
        }
    }

    private async void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        if (!_googleAuthService.IsSupportedOnCurrentPlatform())
        {
            await DisplayAlert("Google Login", "Google login is not supported on Windows. Please test on Android device or emulator.", "OK");
            return;
        }

        try
        {
            var ok = await _googleAuthService.LoginWithGoogleAsync();
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

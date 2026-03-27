namespace NexKart.Pages.Auth;
using NexKart.Models;
using NexKart.Services;

public partial class RegisterPage : ContentPage
{
    private readonly FirebaseService _firebaseService = new();
    private readonly GoogleAuthService _googleAuthService = new();

    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnGoToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Validation", "Please enter email and password.", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Validation", "Password and confirm password do not match.", "OK");
            return;
        }

        try
        {
            await _firebaseService.AddProduct(new Product
            {
                Name = "new_user",
                Quantity = 1,
                Price = 0
            });
            await Navigation.PushAsync(new MainPage());
        }
        catch
        {
            await DisplayAlert("Firebase", "Cannot write to Realtime Database.", "OK");
        }
    }

    private async void OnGoogleSignUpClicked(object sender, EventArgs e)
    {
        if (!_googleAuthService.IsSupportedOnCurrentPlatform())
        {
            await DisplayAlert("Google Sign Up", "Google sign up is not supported on Windows. Please test on Android device or emulator.", "OK");
            return;
        }

        try
        {
            var ok = await _googleAuthService.LoginWithGoogleAsync();
            if (!ok)
            {
                await DisplayAlert("Google Sign Up", "Google sign up failed.", "OK");
                return;
            }

            await Navigation.PushAsync(new MainPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Google Sign Up", ex.Message, "OK");
        }
    }
}

using NexKart.Models;

namespace NexKart.Pages.Auth;

public partial class RegisterPage : ContentPage
{
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
            await DisplayAlert("Validation", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            // Step 1: Create account in Firebase Authentication
            await App.Auth.SignUpAsync(EmailEntry.Text.Trim(), PasswordEntry.Text);

            // Step 2: Save user profile to Realtime Database
            AppUser newUser = new AppUser
            {
                Id = App.Auth.CurrentUserId,
                Email = App.Auth.CurrentUserEmail,
                FullName = FullNameEntry.Text ?? "",
                Phone = "",
                Role = "customer",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await App.Firebase.AddUser(newUser);

            await Navigation.PushAsync(new MainPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Registration Failed", ex.Message, "OK");
        }
    }

    private async void OnGoogleSignUpClicked(object sender, EventArgs e)
    {
        if (!App.GoogleAuth.IsSupportedOnCurrentPlatform())
        {
            await DisplayAlert("Google Sign Up", "Google sign up is not supported on Windows.", "OK");
            return;
        }

        try
        {
            bool ok = await App.GoogleAuth.LoginWithGoogleAsync();
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

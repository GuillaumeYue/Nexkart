namespace NexKart.Pages.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnGoToSignUpTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Login", "Login flow will be connected next.", "OK");
        await Navigation.PushAsync(new MainPage());
    }
}

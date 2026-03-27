namespace NexKart.Pages.Auth;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnGoToLoginTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Sign Up", "Sign up flow will be connected next.", "OK");
        await Navigation.PushAsync(new MainPage());
    }
}

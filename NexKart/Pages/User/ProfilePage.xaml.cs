namespace NexKart.Pages.User;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Profile", "Logout flow will be added next.", "OK");
    }
}

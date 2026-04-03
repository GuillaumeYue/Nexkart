using NexKart.Pages.Auth;

namespace NexKart.Pages.User;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var user = await App.Firebase.GetUserById(App.Auth.CurrentUserId);

        if (user != null)
        {
            NameLabel.Text = string.IsNullOrEmpty(user.FullName) ? "User" : user.FullName;
            EmailLabel.Text = user.Email;
            PhoneLabel.Text = string.IsNullOrEmpty(user.Phone) ? "" : user.Phone;
        }
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfilePage());
    }

    private async void OnMyOrdersTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MyOrdersPage());
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");

        if (confirm)
        {
            App.Auth.SignOut();
            Application.Current.Windows[0].Page = new NavigationPage(new AuthPage());
        }
    }
}

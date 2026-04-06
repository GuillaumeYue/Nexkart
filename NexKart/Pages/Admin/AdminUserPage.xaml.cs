using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminUserPage : ContentPage
{
    public AdminUserPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        List<AppUser> users = await App.Firebase.GetUsers();
        UserList.ItemsSource = users;
    }

    private void OnDashboardTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminDashboardPage());
    }

    private void OnProductTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminProductPage());
    }

    private void OnOrderTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminOrderPage());
    }
}

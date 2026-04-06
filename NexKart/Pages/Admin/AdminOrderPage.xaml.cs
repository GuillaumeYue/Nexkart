using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminOrderPage : ContentPage
{
    public AdminOrderPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        List<Order> orders = await App.Firebase.GetOrders();
        OrderList.ItemsSource = orders;
    }

    private void OnDashboardTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminDashboardPage());
    }

    private void OnProductTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminProductPage());
    }

    private void OnUserTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminUserPage());
    }
}

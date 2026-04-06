using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load counts from Firebase
        List<Product> products = await App.Firebase.GetProducts();
        List<AppUser> users    = await App.Firebase.GetUsers();
        List<Order> orders     = await App.Firebase.GetOrders();

        ProductCountLabel.Text = products.Count.ToString();
        UserCountLabel.Text    = users.Count.ToString();
        OrderCountLabel.Text   = orders.Count.ToString();
    }

    private void OnProductTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminProductPage());
    }

    private void OnUserTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminUserPage());
    }

    private void OnOrderTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminOrderPage());
    }
}

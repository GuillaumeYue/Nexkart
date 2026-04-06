using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminProductPage : ContentPage
{
    public AdminProductPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        List<Product> products = await App.Firebase.GetProducts();
        ProductList.ItemsSource = products;
    }

    private void OnDashboardTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminDashboardPage());
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

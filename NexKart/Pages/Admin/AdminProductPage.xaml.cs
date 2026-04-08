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
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        List<Product> products = await App.Firebase.GetProducts();
        ProductList.ItemsSource = null;
        ProductList.ItemsSource = products;
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminProductFormPage());
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        Border border = (Border)sender;
        Product product = (Product)border.BindingContext;
        await Navigation.PushAsync(new AdminProductFormPage(product));
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        Product product = (Product)btn.BindingContext;

        bool confirm = await DisplayAlert("Delete", "Delete " + product.Name + "?", "Yes", "No");
        if (confirm)
        {
            await App.Firebase.DeleteProduct(product.Id);
            await LoadProducts();
        }
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

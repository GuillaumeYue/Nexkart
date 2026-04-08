using NexKart.Models;

namespace NexKart.Pages.User;

public partial class CartPage : ContentPage
{
    List<CartItem> cartItems = new List<CartItem>();

    public CartPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCart();
    }

    private async Task LoadCart()
    {
        cartItems = await App.Firebase.GetCart(App.Auth.CurrentUserId);
        CartList.ItemsSource = null;
        CartList.ItemsSource = cartItems;
        RefreshTotal();
    }

    private async void OnPlusClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        CartItem item = (CartItem)btn.BindingContext;

        item.Quantity = item.Quantity + 1;
        await App.Firebase.UpdateCartItem(App.Auth.CurrentUserId, item);
        await LoadCart();
    }

    private async void OnMinusClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        CartItem item = (CartItem)btn.BindingContext;

        if (item.Quantity > 1)
        {
            item.Quantity = item.Quantity - 1;
            await App.Firebase.UpdateCartItem(App.Auth.CurrentUserId, item);
        }
        else
        {
            await App.Firebase.RemoveFromCart(App.Auth.CurrentUserId, item.ProductId);
        }

        await LoadCart();
    }

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        CartItem item = (CartItem)btn.BindingContext;

        await App.Firebase.RemoveFromCart(App.Auth.CurrentUserId, item.ProductId);
        await LoadCart();
    }

    private async void OnCheckoutClicked(object sender, EventArgs e)
    {
        if (cartItems.Count == 0)
        {
            await DisplayAlert("Cart", "Your cart is empty.", "OK");
            return;
        }

        // Calculate total
        decimal total = 0;
        foreach (CartItem cartItem in cartItems)
        {
            total = total + (cartItem.Price * cartItem.Quantity);
        }

        // Navigate to payment page
        await Navigation.PushAsync(new PaymentPage(cartItems, total));
    }

    private void RefreshTotal()
    {
        decimal total = 0;

        foreach (CartItem item in cartItems)
        {
            total = total + (item.Price * item.Quantity);
        }

        TotalLabel.Text = "$" + total.ToString("0.00");
    }
}

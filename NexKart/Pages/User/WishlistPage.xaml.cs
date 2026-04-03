using NexKart.Models;

namespace NexKart.Pages.User;

public partial class WishlistPage : ContentPage
{
    public WishlistPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadWishlist();
    }

    private async Task LoadWishlist()
    {
        List<WishlistItem> items = await App.Firebase.GetWishlist(App.Auth.CurrentUserId);

        WishlistList.ItemsSource = null;
        WishlistList.ItemsSource = items;

        // Show empty message if no items
        EmptyLabel.IsVisible = (items.Count == 0);
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        WishlistItem item = (WishlistItem)btn.BindingContext;

        // Add to cart
        CartItem cartItem = new CartItem
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Price = item.Price,
            Quantity = 1
        };

        await App.Firebase.AddToCart(App.Auth.CurrentUserId, cartItem);

        // Remove from wishlist after adding to cart
        await App.Firebase.RemoveFromWishlist(App.Auth.CurrentUserId, item.ProductId);

        await DisplayAlert("Wishlist", item.ProductName + " moved to cart!", "OK");

        await LoadWishlist();
    }

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        WishlistItem item = (WishlistItem)btn.BindingContext;

        await App.Firebase.RemoveFromWishlist(App.Auth.CurrentUserId, item.ProductId);
        await LoadWishlist();
    }
}

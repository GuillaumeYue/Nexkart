using NexKart.Models;

namespace NexKart.Pages.User;

public partial class ProductDetailPage : ContentPage
{
    private Product _product;

    public ProductDetailPage(Product product)
    {
        InitializeComponent();
        _product = product;

        // Set the UI fields from the product
        ProductImage.Source = product.Image;
        ProductName.Text = product.Name;
        ProductPrice.Text = "$" + product.Price.ToString("0.00");
        ProductStock.Text = "In Stock: " + product.Quantity;
        ProductDescription.Text = string.IsNullOrEmpty(product.Description)
            ? "No description available."
            : product.Description;
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        CartItem cartItem = new CartItem
        {
            ProductId = _product.Id,
            ProductName = _product.Name,
            Price = _product.Price,
            Quantity = 1
        };

        await App.Firebase.AddToCart(App.Auth.CurrentUserId, cartItem);
        await DisplayAlert("Cart", _product.Name + " added to cart!", "OK");
    }

    private async void OnAddToWishlistClicked(object sender, EventArgs e)
    {
        WishlistItem wishlistItem = new WishlistItem
        {
            ProductId = _product.Id,
            ProductName = _product.Name,
            Price = _product.Price,
            Image = _product.Image
        };

        bool success = await App.Firebase.AddToWishlist(App.Auth.CurrentUserId, wishlistItem);
        if (success)
            await DisplayAlert("Wishlist", _product.Name + " added to wishlist!", "OK");
        else
            await DisplayAlert("Error", "Failed to add to wishlist.", "OK");
    }
}

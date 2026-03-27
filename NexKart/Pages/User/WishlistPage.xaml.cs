namespace NexKart.Pages.User;

public partial class WishlistPage : ContentPage
{
    public WishlistPage()
    {
        InitializeComponent();
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Wishlist", "Added to cart.", "OK");
    }
}

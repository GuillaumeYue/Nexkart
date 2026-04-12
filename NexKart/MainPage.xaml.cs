using NexKart.Models;
using NexKart.Pages.User;

namespace NexKart
{
    public partial class MainPage : ContentPage
    {
        List<Product> allProducts = new List<Product>();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            allProducts = await App.Firebase.GetProducts();
            ProductList.ItemsSource = allProducts;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = (e.NewTextValue ?? "").Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                ProductList.ItemsSource = allProducts;
            }
            else
            {
                List<Product> filtered = new List<Product>();
                foreach (Product p in allProducts)
                {
                    if (p.Name.ToLower().Contains(keyword))
                        filtered.Add(p);
                }
                ProductList.ItemsSource = filtered;
            }
        }

        private async void OnProductTapped(object sender, TappedEventArgs e)
        {
            Border border = (Border)sender;
            Product product = (Product)border.BindingContext;
            await Navigation.PushAsync(new ProductDetailPage(product));
        }

        private async void OnAddToCartClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Product product = (Product)btn.BindingContext;

            CartItem cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1
            };

            await App.Firebase.AddToCart(App.Auth.CurrentUserId, cartItem);
            await DisplayAlert("Cart", product.Name + " added to cart!", "OK");
        }

        private async void OnAddToWishlistClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Product product = (Product)btn.BindingContext;

            WishlistItem wishlistItem = new WishlistItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Image = product.Image
            };

            bool success = await App.Firebase.AddToWishlist(App.Auth.CurrentUserId, wishlistItem);
            if (success)
                await DisplayAlert("Wishlist", product.Name + " added to wishlist!", "OK");
            else
                await DisplayAlert("Error", "Failed to add to wishlist.", "OK");
        }

        private async void OnCartTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new CartPage());
        }

        private async void OnWishlistTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new WishlistPage());
        }

        private async void OnProfileTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }
    }
}

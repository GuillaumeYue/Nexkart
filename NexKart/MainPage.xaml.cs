namespace NexKart
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCartTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new Pages.User.CartPage());
        }

        private async void OnWishlistTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new Pages.User.WishlistPage());
        }

        private async void OnProfileTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new Pages.User.ProfilePage());
        }
    }
}

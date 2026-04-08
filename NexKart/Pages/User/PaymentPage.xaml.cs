using NexKart.Models;

namespace NexKart.Pages.User;

public partial class PaymentPage : ContentPage
{
    private List<CartItem> _cartItems;
    private decimal _total;

    public PaymentPage(List<CartItem> cartItems, decimal total)
    {
        InitializeComponent();
        _cartItems = cartItems;
        _total = total;

        ItemCountLabel.Text = cartItems.Count + " item(s)";
        TotalLabel.Text = "$" + total.ToString("0.00");
    }

    private async void OnPayClicked(object sender, EventArgs e)
    {
        string cardNumber = CardNumberEntry.Text?.Trim() ?? "";
        string expMonth = ExpMonthEntry.Text?.Trim() ?? "";
        string expYear = ExpYearEntry.Text?.Trim() ?? "";
        string cvc = CvcEntry.Text?.Trim() ?? "";

        // Validate inputs
        if (cardNumber.Length < 13)
        {
            await DisplayAlert("Error", "Please enter a valid card number.", "OK");
            return;
        }

        // Show loading
        PayButton.IsEnabled = false;
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            // Step 1: Create PaymentIntent
            string clientSecret = await App.Stripe.CreatePaymentIntent(_total);

            // Step 2: Confirm payment with test token
            bool success = await App.Stripe.ConfirmPayment(clientSecret, cardNumber);

            if (success)
            {
                // Step 3: Create order in Firebase
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (CartItem cartItem in _cartItems)
                {
                    orderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price
                    });
                }

                Order order = new Order
                {
                    UserId = App.Auth.CurrentUserId,
                    Status = "Paid",
                    TotalAmount = _total,
                    CreatedAt = DateTime.UtcNow,
                    Items = orderItems
                };

                await App.Firebase.AddOrder(order);

                // Step 4: Clear cart
                await App.Firebase.ClearCart(App.Auth.CurrentUserId);

                await DisplayAlert("Success", "Payment successful! Your order has been placed.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Failed", "Payment was not successful. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Payment error: " + ex.Message, "OK");
        }
        finally
        {
            PayButton.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}

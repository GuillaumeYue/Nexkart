using NexKart.Models;

namespace NexKart.Pages.User;

public partial class MyOrdersPage : ContentPage
{
    public MyOrdersPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        try
        {
            List<Order> orders = await App.Firebase.GetOrdersByUser(App.Auth.CurrentUserId);

            // Build a display list with item summary text
            List<OrderDisplay> displayList = new List<OrderDisplay>();
            foreach (Order order in orders)
            {
                OrderDisplay display = new OrderDisplay();
                display.CreatedAt = order.CreatedAt;
                display.Status = order.Status;
                display.TotalAmount = order.TotalAmount;

                // Build summary like "Laptop x2, Mouse x1"
                List<string> parts = new List<string>();
                foreach (OrderItem item in order.Items)
                {
                    parts.Add(item.ProductName + " x" + item.Quantity);
                }
                display.ItemsSummary = string.Join(", ", parts);

                displayList.Add(display);
            }

            OrderList.ItemsSource = null;
            OrderList.ItemsSource = displayList;

            EmptyLabel.IsVisible = (displayList.Count == 0);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load orders: " + ex.Message, "OK");
        }
    }
}

// Simple display class for binding in the UI
public class OrderDisplay
{
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string ItemsSummary { get; set; } = "";
}

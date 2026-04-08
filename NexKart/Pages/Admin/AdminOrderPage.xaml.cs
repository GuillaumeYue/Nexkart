using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminOrderPage : ContentPage
{
    private List<Order> _orders = new List<Order>();

    public AdminOrderPage()
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
        _orders = await App.Firebase.GetOrders();

        List<AdminOrderDisplay> displayList = new List<AdminOrderDisplay>();
        foreach (Order order in _orders)
        {
            // Build items summary
            List<string> parts = new List<string>();
            foreach (OrderItem item in order.Items)
            {
                parts.Add(item.ProductName + " x" + item.Quantity);
            }

            displayList.Add(new AdminOrderDisplay
            {
                Id = order.Id,
                ItemsSummary = parts.Count > 0 ? string.Join(", ", parts) : "No items",
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            });
        }

        OrderList.ItemsSource = null;
        OrderList.ItemsSource = displayList;
    }

    private async void OnOrderTapped(object sender, TappedEventArgs e)
    {
        Border border = (Border)sender;
        AdminOrderDisplay display = (AdminOrderDisplay)border.BindingContext;

        // Find the full order
        Order order = null;
        foreach (Order o in _orders)
        {
            if (o.Id == display.Id)
            {
                order = o;
                break;
            }
        }

        if (order != null)
        {
            await Navigation.PushAsync(new AdminOrderDetailPage(order));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        AdminOrderDisplay display = (AdminOrderDisplay)btn.BindingContext;

        bool confirm = await DisplayAlert("Delete", "Delete this order?", "Yes", "No");
        if (confirm)
        {
            await App.Firebase.DeleteOrder(display.Id);
            await LoadOrders();
        }
    }

    private void OnDashboardTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminDashboardPage());
    }

    private void OnProductTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminProductPage());
    }

    private void OnUserTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminUserPage());
    }
}

public class AdminOrderDisplay
{
    public string Id { get; set; } = "";
    public string ItemsSummary { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

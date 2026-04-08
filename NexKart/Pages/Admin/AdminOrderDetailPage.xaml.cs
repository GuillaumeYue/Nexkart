using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminOrderDetailPage : ContentPage
{
    private Order _order;

    public AdminOrderDetailPage(Order order)
    {
        InitializeComponent();
        _order = order;

        OrderIdLabel.Text = "Order: " + order.Id;
        DateLabel.Text = "Date: " + order.CreatedAt.ToString("MMM dd, yyyy HH:mm");
        TotalLabel.Text = "Total: $" + order.TotalAmount.ToString("0.00");

        // Set status picker
        for (int i = 0; i < StatusPicker.Items.Count; i++)
        {
            if (StatusPicker.Items[i] == order.Status)
            {
                StatusPicker.SelectedIndex = i;
                break;
            }
        }

        // Show items
        ItemList.ItemsSource = order.Items;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string newStatus = StatusPicker.SelectedItem?.ToString() ?? "Pending";

        _order.Status = newStatus;

        try
        {
            await App.Firebase.UpdateOrder(_order);
            await DisplayAlert("Success", "Order status updated!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to update: " + ex.Message, "OK");
        }
    }
}

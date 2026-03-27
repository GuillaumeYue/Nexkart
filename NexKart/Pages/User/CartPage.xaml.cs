namespace NexKart.Pages.User;

public partial class CartPage : ContentPage
{
    private int _qty1 = 1;
    private int _qty2 = 1;
    private const decimal Price1 = 199m;
    private const decimal Price2 = 249m;

    public CartPage()
    {
        InitializeComponent();
        RefreshSummary();
    }

    private void OnMinusClicked(object sender, EventArgs e)
    {
        if (_qty1 > 1) _qty1--;
        RefreshSummary();
    }

    private void OnPlusClicked(object sender, EventArgs e)
    {
        _qty1++;
        RefreshSummary();
    }

    private void OnMinus2Clicked(object sender, EventArgs e)
    {
        if (_qty2 > 1) _qty2--;
        RefreshSummary();
    }

    private void OnPlus2Clicked(object sender, EventArgs e)
    {
        _qty2++;
        RefreshSummary();
    }

    private async void OnCheckoutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Checkout", "Checkout page will be added next.", "OK");
    }

    private void RefreshSummary()
    {
        Qty1Label.Text = _qty1.ToString();
        Qty2Label.Text = _qty2.ToString();
        var total = (_qty1 * Price1) + (_qty2 * Price2);
        TotalLabel.Text = $"${total:0.00}";
    }
}

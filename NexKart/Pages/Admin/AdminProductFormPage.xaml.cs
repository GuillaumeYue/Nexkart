using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminProductFormPage : ContentPage
{
    private Product _existing;
    private bool _isEdit;

    // Add mode
    public AdminProductFormPage()
    {
        InitializeComponent();
        _isEdit = false;
    }

    // Edit mode
    public AdminProductFormPage(Product product)
    {
        InitializeComponent();
        _isEdit = true;
        _existing = product;

        PageTitle.Text = "Edit Product";
        SaveButton.Text = "Save Changes";

        NameEntry.Text = product.Name;
        DescriptionEditor.Text = product.Description;
        CategoryPicker.SelectedItem = product.Category;
        ImageEntry.Text = product.Image;
        PriceEntry.Text = product.Price.ToString("0.00");
        QuantityEntry.Text = product.Quantity.ToString();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim() ?? "";
        string description = DescriptionEditor.Text?.Trim() ?? "";
        string category = CategoryPicker.SelectedItem?.ToString() ?? "";
        string image = ImageEntry.Text?.Trim() ?? "";
        string priceText = PriceEntry.Text?.Trim() ?? "";
        string qtyText = QuantityEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Error", "Product name is required.", "OK");
            return;
        }

        decimal price = 0;
        if (!decimal.TryParse(priceText, out price) || price < 0)
        {
            await DisplayAlert("Error", "Please enter a valid price.", "OK");
            return;
        }

        int quantity = 0;
        if (!int.TryParse(qtyText, out quantity) || quantity < 0)
        {
            await DisplayAlert("Error", "Please enter a valid quantity.", "OK");
            return;
        }

        try
        {
            if (_isEdit)
            {
                _existing.Name = name;
                _existing.Description = description;
                _existing.Category = category;
                _existing.Image = image;
                _existing.Price = price;
                _existing.Quantity = quantity;

                await App.Firebase.UpdateProduct(_existing);
                await DisplayAlert("Success", "Product updated!", "OK");
            }
            else
            {
                Product product = new Product();
                product.Name = name;
                product.Description = description;
                product.Category = category;
                product.Image = image;
                product.Price = price;
                product.Quantity = quantity;

                await App.Firebase.AddProduct(product);
                await DisplayAlert("Success", "Product added!", "OK");
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to save: " + ex.Message, "OK");
        }
    }
}

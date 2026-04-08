using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminUserFormPage : ContentPage
{
    private AppUser _user;

    public AdminUserFormPage(AppUser user)
    {
        InitializeComponent();
        _user = user;

        EmailLabel.Text = user.Email;
        NameEntry.Text = user.FullName;
        PhoneEntry.Text = user.Phone;
        ActiveSwitch.IsToggled = user.IsActive;

        // Set role picker
        if (user.Role == "admin")
            RolePicker.SelectedIndex = 1;
        else
            RolePicker.SelectedIndex = 0;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim() ?? "";
        string phone = PhoneEntry.Text?.Trim() ?? "";
        string role = RolePicker.SelectedItem?.ToString() ?? "customer";
        bool isActive = ActiveSwitch.IsToggled;

        _user.FullName = name;
        _user.Phone = phone;
        _user.Role = role;
        _user.IsActive = isActive;

        try
        {
            await App.Firebase.UpdateUser(_user);
            await DisplayAlert("Success", "User updated!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to save: " + ex.Message, "OK");
        }
    }
}

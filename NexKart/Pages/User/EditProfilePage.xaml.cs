using NexKart.Models;

namespace NexKart.Pages.User;

public partial class EditProfilePage : ContentPage
{
    private AppUser _user;

    public EditProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _user = await App.Firebase.GetUserById(App.Auth.CurrentUserId);

        if (_user != null)
        {
            NameEntry.Text = _user.FullName;
            PhoneEntry.Text = _user.Phone;
            EmailLabel.Text = _user.Email;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_user == null) return;

        string name = NameEntry.Text?.Trim() ?? "";
        string phone = PhoneEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Error", "Name cannot be empty.", "OK");
            return;
        }

        _user.FullName = name;
        _user.Phone = phone;

        try
        {
            await App.Firebase.UpdateUser(_user);
            await DisplayAlert("Success", "Profile updated!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to save: " + ex.Message, "OK");
        }
    }
}

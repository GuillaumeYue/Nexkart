using NexKart.Models;

namespace NexKart.Pages.Admin;

public partial class AdminUserPage : ContentPage
{
    public AdminUserPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        List<AppUser> users = await App.Firebase.GetUsers();

        // Build display list with active status text
        List<UserDisplay> displayList = new List<UserDisplay>();
        foreach (AppUser user in users)
        {
            displayList.Add(new UserDisplay
            {
                Id = user.Id,
                FullName = string.IsNullOrEmpty(user.FullName) ? "No Name" : user.FullName,
                Email = user.Email,
                Role = user.Role,
                ActiveStatus = user.IsActive ? "Active" : "Inactive"
            });
        }

        UserList.ItemsSource = null;
        UserList.ItemsSource = displayList;
    }

    private async void OnUserTapped(object sender, TappedEventArgs e)
    {
        Border border = (Border)sender;
        UserDisplay display = (UserDisplay)border.BindingContext;

        // Load full user data for editing
        AppUser user = await App.Firebase.GetUserById(display.Id);
        if (user != null)
        {
            await Navigation.PushAsync(new AdminUserFormPage(user));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        UserDisplay display = (UserDisplay)btn.BindingContext;

        bool confirm = await DisplayAlert("Delete", "Delete user " + display.FullName + "?", "Yes", "No");
        if (confirm)
        {
            await App.Firebase.DeleteUser(display.Id);
            await LoadUsers();
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

    private void OnOrderTabTapped(object sender, TappedEventArgs e)
    {
        Application.Current.Windows[0].Page = new NavigationPage(new AdminOrderPage());
    }
}

public class UserDisplay
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string ActiveStatus { get; set; } = "";
}

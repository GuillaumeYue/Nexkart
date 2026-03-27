namespace NexKart.Pages;

public partial class SplashScreen : ContentPage
{
    public SplashScreen()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(4000);

        if (Application.Current?.Windows.FirstOrDefault() is Window window)
        {
            window.Page = new NavigationPage(new Auth.AuthPage());
        }
    }
}

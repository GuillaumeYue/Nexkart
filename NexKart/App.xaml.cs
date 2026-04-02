using NexKart.Pages;
using NexKart.Services;

namespace NexKart
{
    public partial class App : Application
    {
        // Global services - accessible from anywhere with App.Auth, App.Firebase, App.GoogleAuth
        public static AuthService Auth { get; } = new AuthService();
        public static FirebaseService Firebase { get; } = new FirebaseService();
        public static GoogleAuthService GoogleAuth { get; } = new GoogleAuthService();

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashScreen());
        }
    }
}

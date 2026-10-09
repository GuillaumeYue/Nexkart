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
        public static StripeService Stripe { get; } = new StripeService();

        public App()
        {
            InitializeComponent();
            // Admin accounts are no longer created from the app with hard-coded credentials.
            // To make a user an admin, set Role = "admin" on their document in the Firestore
            // "Users" collection (see README.md).
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashScreen());
        }
    }
}

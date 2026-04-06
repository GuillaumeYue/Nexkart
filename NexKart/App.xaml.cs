using NexKart.Models;
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
            SeedAdminAsync();
        }

        private async void SeedAdminAsync()
        {
            const string adminEmail    = "guillaume@gmail.com";
            const string adminPassword = "12345678";

            try
            {
                // Try to sign in — if it works, the account already exists
                await Auth.SignInAsync(adminEmail, adminPassword);

                // Make sure the Firestore document has Role = "admin"
                AppUser existing = await Firebase.GetUserById(Auth.CurrentUserId);
                if (existing == null || existing.Role != "admin")
                {
                    AppUser admin = new AppUser();
                    admin.Id       = Auth.CurrentUserId;
                    admin.Email    = adminEmail;
                    admin.FullName = "Guillaume";
                    admin.Role     = "admin";
                    admin.IsActive = true;
                    admin.CreatedAt = DateTime.UtcNow;
                    await Firebase.AddUser(admin);
                }

                Auth.SignOut();
            }
            catch
            {
                // Account doesn't exist yet — create it
                try
                {
                    await Auth.SignUpAsync(adminEmail, adminPassword);

                    AppUser admin = new AppUser();
                    admin.Id       = Auth.CurrentUserId;
                    admin.Email    = adminEmail;
                    admin.FullName = "Guillaume";
                    admin.Role     = "admin";
                    admin.IsActive = true;
                    admin.CreatedAt = DateTime.UtcNow;
                    await Firebase.AddUser(admin);

                    Auth.SignOut();
                }
                catch
                {
                    // Already handled or Firebase unreachable — skip silently
                }
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashScreen());
        }
    }
}

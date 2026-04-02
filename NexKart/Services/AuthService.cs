using Firebase.Auth;
using Firebase.Auth.Providers;

namespace NexKart.Services;

public class AuthService
{
    private FirebaseAuthClient _client;
    private UserCredential _currentUser;

    public string CurrentUserId { get; private set; } = "";
    public string CurrentUserEmail { get; private set; } = "";
    public bool IsSignedIn { get; private set; } = false;

    public AuthService()
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyAzAGWCSGFapeL5Y9v623xnQPM-sWRTxEg",
            AuthDomain = "nexkart-65149.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        _client = new FirebaseAuthClient(config);
    }

    public async Task SignInAsync(string email, string password)
    {
        _currentUser = await _client.SignInWithEmailAndPasswordAsync(email, password);
        CurrentUserId = _currentUser.User.Uid;
        CurrentUserEmail = _currentUser.User.Info.Email;
        IsSignedIn = true;
    }

    public async Task SignUpAsync(string email, string password)
    {
        _currentUser = await _client.CreateUserWithEmailAndPasswordAsync(email, password);
        CurrentUserId = _currentUser.User.Uid;
        CurrentUserEmail = _currentUser.User.Info.Email;
        IsSignedIn = true;
    }

    public void SignOut()
    {
        _currentUser = null;
        CurrentUserId = "";
        CurrentUserEmail = "";
        IsSignedIn = false;
    }

    public async Task<string> GetIdTokenAsync()
    {
        if (_currentUser == null)
        {
            return "";
        }

        return await _currentUser.User.GetIdTokenAsync();
    }
}

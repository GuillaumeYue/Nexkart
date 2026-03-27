using System.Text;
using System.Text.Json;
using Microsoft.Maui.Authentication;

namespace NexKart.Services;

public class GoogleAuthService
{
    private const string GoogleWebClientId = "584541222042-nuooa51lfv1soeaubqpsqf7p183uv19.apps.googleusercontent.com";
    private const string FirebaseApiKey = "AIzaSyAkeA41s1T7756U4AS5GhJ4kShc9PpBacs";
    private const string RedirectUri = "nexkart://auth";

    private readonly HttpClient _httpClient = new();

    public bool IsSupportedOnCurrentPlatform()
    {
        return DeviceInfo.Current.Platform != DevicePlatform.WinUI;
    }

    public async Task<bool> LoginWithGoogleAsync()
    {
        if (!IsSupportedOnCurrentPlatform())
        {
            return false;
        }

        var nonce = Guid.NewGuid().ToString("N");
        var state = Guid.NewGuid().ToString("N");

        var authUrl =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={Uri.EscapeDataString(GoogleWebClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
            "&response_type=id_token" +
            "&scope=" + Uri.EscapeDataString("openid email profile") +
            $"&nonce={Uri.EscapeDataString(nonce)}" +
            $"&state={Uri.EscapeDataString(state)}" +
            "&response_mode=fragment";

        var authResult = await WebAuthenticator.Default.AuthenticateAsync(
            new Uri(authUrl),
            new Uri(RedirectUri));

        if (!authResult.Properties.TryGetValue("id_token", out var googleIdToken) || string.IsNullOrWhiteSpace(googleIdToken))
        {
            return false;
        }

        return await SignInToFirebaseWithGoogleAsync(googleIdToken);
    }

    private async Task<bool> SignInToFirebaseWithGoogleAsync(string googleIdToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={FirebaseApiKey}";
        var payload = new
        {
            postBody = $"id_token={googleIdToken}&providerId=google.com",
            requestUri = RedirectUri,
            returnIdpCredential = true,
            returnSecureToken = true
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        return response.IsSuccessStatusCode;
    }
}

using System.Net.Http.Headers;
using System.Text.Json;

namespace NexKart.Services;

public class StripeService
{
    private const string BaseUrl = "https://api.stripe.com/v1";

    private HttpClient _client;
    private bool _initialized = false;

    public StripeService()
    {
        _client = new HttpClient();
    }

    private async Task EnsureInitialized()
    {
        if (_initialized) return;

        using Stream stream = await FileSystem.OpenAppPackageFileAsync("stripe_key.txt");
        using StreamReader reader = new StreamReader(stream);
        string secretKey = (await reader.ReadToEndAsync()).Trim();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", secretKey);
        _initialized = true;
    }

    // Create a PaymentIntent with amount in cents
    public async Task<string> CreatePaymentIntent(decimal amount, string currency = "usd")
    {
        await EnsureInitialized();
        int amountInCents = (int)(amount * 100);

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("amount", amountInCents.ToString()),
            new KeyValuePair<string, string>("currency", currency),
            new KeyValuePair<string, string>("payment_method_types[]", "card")
        });

        HttpResponseMessage response = await _client.PostAsync(BaseUrl + "/payment_intents", content);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to create payment: " + json);

        JsonDocument doc = JsonDocument.Parse(json);
        string clientSecret = doc.RootElement.GetProperty("client_secret").GetString() ?? "";
        return clientSecret;
    }

    // Confirm payment using Stripe test payment method tokens
    public async Task<bool> ConfirmPayment(string clientSecret, string cardNumber)
    {
        await EnsureInitialized();
        // Extract PaymentIntent ID from client secret (format: pi_xxx_secret_xxx)
        string paymentIntentId = clientSecret.Split("_secret_")[0];

        // Map test card numbers to Stripe test payment method tokens
        string paymentMethod = "pm_card_visa"; // default
        string cleanCard = cardNumber.Replace(" ", "");

        if (cleanCard == "4242424242424242")
            paymentMethod = "pm_card_visa";
        else if (cleanCard == "5555555555554444")
            paymentMethod = "pm_card_mastercard";
        else if (cleanCard == "4000000000009995")
            paymentMethod = "pm_card_visa_chargeDeclined";
        else if (cleanCard == "4000000000000002")
            paymentMethod = "pm_card_visa_chargeDeclined";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("payment_method", paymentMethod)
        });

        string url = BaseUrl + "/payment_intents/" + paymentIntentId + "/confirm";
        HttpResponseMessage response = await _client.PostAsync(url, content);
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception("Payment failed: " + json);

        JsonDocument doc = JsonDocument.Parse(json);
        string status = doc.RootElement.GetProperty("status").GetString() ?? "";

        return status == "succeeded";
    }
}

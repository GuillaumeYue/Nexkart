using System.Net.Http.Headers;
using System.Text.Json;

namespace NexKart.Services;

public class StripeService
{
    private const string SecretKey = "YOUR_STRIPE_SECRET_KEY";
    private const string BaseUrl = "https://api.stripe.com/v1";

    private HttpClient _client;

    public StripeService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", SecretKey);
    }

    // Create a PaymentIntent with amount in cents
    public async Task<string> CreatePaymentIntent(decimal amount, string currency = "usd")
    {
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

    // Confirm payment with card details (test mode)
    public async Task<bool> ConfirmPayment(string clientSecret, string cardNumber, string expMonth, string expYear, string cvc)
    {
        // Extract PaymentIntent ID from client secret (format: pi_xxx_secret_xxx)
        string paymentIntentId = clientSecret.Split("_secret_")[0];

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("payment_method_data[type]", "card"),
            new KeyValuePair<string, string>("payment_method_data[card][number]", cardNumber),
            new KeyValuePair<string, string>("payment_method_data[card][exp_month]", expMonth),
            new KeyValuePair<string, string>("payment_method_data[card][exp_year]", expYear),
            new KeyValuePair<string, string>("payment_method_data[card][cvc]", cvc)
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

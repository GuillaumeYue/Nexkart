using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NexKart.Models;

namespace NexKart.Services;

public class FirebaseService
{
    private const string ProjectId = "nexkart-65149";
    private const string BaseUrl = "https://firestore.googleapis.com/v1/projects/" + ProjectId + "/databases/(default)/documents";

    private HttpClient _httpClient = new HttpClient();

    // Attach the Firebase Auth token to every request
    private async Task SetAuthHeader()
    {
        string token = await App.Auth.GetIdTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    // Deserialize options — lets Firestore's lowercase JSON match our C# class properties
    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    // ==================== Firestore value helpers ====================
    // Firestore wraps every value in a type object like {"stringValue": "hello"}
    // These helpers just pull the actual value out of that wrapper.

    private string GetString(Dictionary<string, FirestoreValue> fields, string key)
    {
        if (fields.ContainsKey(key) && fields[key].stringValue != null)
        {
            return fields[key].stringValue;
        }
        return "";
    }

    private decimal GetDecimal(Dictionary<string, FirestoreValue> fields, string key)
    {
        if (fields.ContainsKey(key))
        {
            FirestoreValue val = fields[key];
            if (val.doubleValue != null)
            {
                return (decimal)val.doubleValue;
            }
            if (val.integerValue != null)
            {
                return decimal.Parse(val.integerValue);
            }
        }
        return 0;
    }

    private int GetInt(Dictionary<string, FirestoreValue> fields, string key)
    {
        if (fields.ContainsKey(key) && fields[key].integerValue != null)
        {
            return int.Parse(fields[key].integerValue);
        }
        return 0;
    }

    private bool GetBool(Dictionary<string, FirestoreValue> fields, string key)
    {
        if (fields.ContainsKey(key) && fields[key].booleanValue != null)
        {
            return (bool)fields[key].booleanValue;
        }
        return false;
    }

    private DateTime GetTimestamp(Dictionary<string, FirestoreValue> fields, string key)
    {
        if (fields.ContainsKey(key) && fields[key].timestampValue != null)
        {
            return DateTime.Parse(fields[key].timestampValue);
        }
        return DateTime.UtcNow;
    }

    // Firestore returns the full path as the document name, e.g. "projects/.../documents/Products/abc123"
    // Split by "/" and take the last part to get just the document ID
    private string GetDocId(FirestoreDocument doc)
    {
        string[] parts = doc.name.Split('/');
        return parts[parts.Length - 1];
    }

    // ==================== Product ====================

    private string ProductToJson(Product p)
    {
        return JsonSerializer.Serialize(new
        {
            fields = new
            {
                Name        = new { stringValue  = p.Name },
                Image       = new { stringValue  = p.Image },
                Description = new { stringValue  = p.Description },
                Price       = new { doubleValue  = (double)p.Price },
                Quantity    = new { integerValue = p.Quantity.ToString() }
            }
        });
    }

    private Product FirestoreToProduct(FirestoreDocument doc)
    {
        Dictionary<string, FirestoreValue> f = doc.fields;

        Product product = new Product();
        product.Id          = GetDocId(doc);
        product.Name        = GetString(f, "Name");
        product.Image       = GetString(f, "Image");
        product.Description = GetString(f, "Description");
        product.Price       = GetDecimal(f, "Price");
        product.Quantity    = GetInt(f, "Quantity");

        return product;
    }

    public async Task<List<Product>> GetProducts()
    {
        await SetAuthHeader();

        var response = await _httpClient.GetAsync(BaseUrl + "/Products");
        string json = await response.Content.ReadAsStringAsync();

        FirestoreList list = JsonSerializer.Deserialize<FirestoreList>(json, _jsonOptions);

        List<Product> result = new List<Product>();
        foreach (FirestoreDocument doc in list.documents)
        {
            result.Add(FirestoreToProduct(doc));
        }

        return result;
    }

    public async Task AddProduct(Product product)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(ProductToJson(product), Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(BaseUrl + "/Products", content);
    }

    public async Task UpdateProduct(Product product)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(ProductToJson(product), Encoding.UTF8, "application/json");
        await _httpClient.PatchAsync(BaseUrl + "/Products/" + product.Id, content);
    }

    public async Task DeleteProduct(string id)
    {
        await SetAuthHeader();
        await _httpClient.DeleteAsync(BaseUrl + "/Products/" + id);
    }

    // ==================== Wishlist (stored under Users/{uid}/Wishlist) ====================

    private string WishlistPath(string userId)
    {
        return BaseUrl + "/Users/" + userId + "/Wishlist";
    }

    private string WishlistItemToJson(WishlistItem item)
    {
        return JsonSerializer.Serialize(new
        {
            fields = new
            {
                ProductId   = new { stringValue = item.ProductId },
                ProductName = new { stringValue = item.ProductName },
                Price       = new { doubleValue = (double)item.Price },
                Image       = new { stringValue = item.Image }
            }
        });
    }

    private WishlistItem FirestoreToWishlistItem(FirestoreDocument doc)
    {
        Dictionary<string, FirestoreValue> f = doc.fields;

        WishlistItem item = new WishlistItem();
        item.ProductId   = GetDocId(doc);
        item.ProductName = GetString(f, "ProductName");
        item.Price       = GetDecimal(f, "Price");
        item.Image       = GetString(f, "Image");

        return item;
    }

    public async Task AddToWishlist(string userId, WishlistItem item)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(WishlistItemToJson(item), Encoding.UTF8, "application/json");
        await _httpClient.PatchAsync(WishlistPath(userId) + "/" + item.ProductId, content);
    }

    public async Task<List<WishlistItem>> GetWishlist(string userId)
    {
        await SetAuthHeader();

        var response = await _httpClient.GetAsync(WishlistPath(userId));

        // If the subcollection doesn't exist yet, Firestore returns 404
        if (!response.IsSuccessStatusCode)
        {
            return new List<WishlistItem>();
        }

        string json = await response.Content.ReadAsStringAsync();

        FirestoreList list = JsonSerializer.Deserialize<FirestoreList>(json, _jsonOptions);

        List<WishlistItem> result = new List<WishlistItem>();
        if (list != null && list.documents != null)
        {
            foreach (FirestoreDocument doc in list.documents)
            {
                result.Add(FirestoreToWishlistItem(doc));
            }
        }

        return result;
    }

    public async Task RemoveFromWishlist(string userId, string productId)
    {
        await SetAuthHeader();
        await _httpClient.DeleteAsync(WishlistPath(userId) + "/" + productId);
    }

    // ==================== Cart (stored under Users/{uid}/Cart) ====================

    private string CartPath(string userId)
    {
        return BaseUrl + "/Users/" + userId + "/Cart";
    }

    private string CartItemToJson(CartItem item)
    {
        return JsonSerializer.Serialize(new
        {
            fields = new
            {
                ProductId   = new { stringValue  = item.ProductId },
                ProductName = new { stringValue  = item.ProductName },
                Price       = new { doubleValue  = (double)item.Price },
                Quantity    = new { integerValue = item.Quantity.ToString() }
            }
        });
    }

    private CartItem FirestoreToCartItem(FirestoreDocument doc)
    {
        Dictionary<string, FirestoreValue> f = doc.fields;

        CartItem item = new CartItem();
        item.ProductId   = GetDocId(doc);
        item.ProductName = GetString(f, "ProductName");
        item.Price       = GetDecimal(f, "Price");
        item.Quantity    = GetInt(f, "Quantity");

        return item;
    }

    public async Task AddToCart(string userId, CartItem item)
    {
        await SetAuthHeader();

        // Check if the product is already in the cart
        var response = await _httpClient.GetAsync(CartPath(userId) + "/" + item.ProductId);
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            FirestoreDocument existingDoc = JsonSerializer.Deserialize<FirestoreDocument>(json, _jsonOptions);
            CartItem existing = FirestoreToCartItem(existingDoc);
            item.Quantity = existing.Quantity + item.Quantity;
        }

        StringContent content = new StringContent(CartItemToJson(item), Encoding.UTF8, "application/json");
        await _httpClient.PatchAsync(CartPath(userId) + "/" + item.ProductId, content);
    }

    public async Task<List<CartItem>> GetCart(string userId)
    {
        await SetAuthHeader();

        var response = await _httpClient.GetAsync(CartPath(userId));
        string json = await response.Content.ReadAsStringAsync();

        FirestoreList list = JsonSerializer.Deserialize<FirestoreList>(json, _jsonOptions);

        List<CartItem> result = new List<CartItem>();
        foreach (FirestoreDocument doc in list.documents)
        {
            result.Add(FirestoreToCartItem(doc));
        }

        return result;
    }

    public async Task UpdateCartItem(string userId, CartItem item)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(CartItemToJson(item), Encoding.UTF8, "application/json");
        await _httpClient.PatchAsync(CartPath(userId) + "/" + item.ProductId, content);
    }

    public async Task RemoveFromCart(string userId, string productId)
    {
        await SetAuthHeader();
        await _httpClient.DeleteAsync(CartPath(userId) + "/" + productId);
    }

    public async Task ClearCart(string userId)
    {
        // Firestore doesn't support deleting a whole collection in one call
        // So we get all items and delete them one by one
        List<CartItem> items = await GetCart(userId);
        foreach (CartItem item in items)
        {
            await RemoveFromCart(userId, item.ProductId);
        }
    }

    // ==================== Order ====================

    private string OrderToJson(Order order)
    {
        // Build the Items list in Firestore's array-of-maps format
        List<object> itemValues = new List<object>();
        foreach (OrderItem item in order.Items)
        {
            itemValues.Add(new
            {
                mapValue = new
                {
                    fields = new
                    {
                        ProductId   = new { stringValue  = item.ProductId },
                        ProductName = new { stringValue  = item.ProductName },
                        Quantity    = new { integerValue = item.Quantity.ToString() },
                        UnitPrice   = new { doubleValue  = (double)item.UnitPrice }
                    }
                }
            });
        }

        return JsonSerializer.Serialize(new
        {
            fields = new
            {
                UserId      = new { stringValue    = order.UserId },
                Status      = new { stringValue    = order.Status },
                TotalAmount = new { doubleValue    = (double)order.TotalAmount },
                CreatedAt   = new { timestampValue = order.CreatedAt.ToString("O") },
                Items       = new { arrayValue = new { values = itemValues } }
            }
        });
    }

    private Order FirestoreToOrder(FirestoreDocument doc)
    {
        Dictionary<string, FirestoreValue> f = doc.fields;

        // Parse the nested OrderItems array
        List<OrderItem> items = new List<OrderItem>();

        if (f.ContainsKey("Items") && f["Items"].arrayValue != null)
        {
            foreach (FirestoreValue val in f["Items"].arrayValue.values)
            {
                if (val.mapValue != null && val.mapValue.fields != null)
                {
                    Dictionary<string, FirestoreValue> mf = val.mapValue.fields;

                    OrderItem orderItem = new OrderItem();
                    orderItem.ProductId   = GetString(mf, "ProductId");
                    orderItem.ProductName = GetString(mf, "ProductName");
                    orderItem.Quantity    = GetInt(mf, "Quantity");
                    orderItem.UnitPrice   = GetDecimal(mf, "UnitPrice");

                    items.Add(orderItem);
                }
            }
        }

        Order order = new Order();
        order.Id          = GetDocId(doc);
        order.UserId      = GetString(f, "UserId");
        order.Status      = GetString(f, "Status");
        order.TotalAmount = GetDecimal(f, "TotalAmount");
        order.CreatedAt   = GetTimestamp(f, "CreatedAt");
        order.Items       = items;

        return order;
    }

    public async Task AddOrder(Order order)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(OrderToJson(order), Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(BaseUrl + "/Orders", content);
    }

    public async Task<List<Order>> GetOrders()
    {
        await SetAuthHeader();

        var response = await _httpClient.GetAsync(BaseUrl + "/Orders");
        string json = await response.Content.ReadAsStringAsync();

        FirestoreList list = JsonSerializer.Deserialize<FirestoreList>(json, _jsonOptions);

        List<Order> result = new List<Order>();
        foreach (FirestoreDocument doc in list.documents)
        {
            result.Add(FirestoreToOrder(doc));
        }

        return result;
    }

    public async Task<List<Order>> GetOrdersByUser(string userId)
    {
        List<Order> all = await GetOrders();
        List<Order> result = new List<Order>();

        foreach (Order order in all)
        {
            if (order.UserId == userId)
            {
                result.Add(order);
            }
        }

        return result;
    }

    public async Task UpdateOrder(Order order)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(OrderToJson(order), Encoding.UTF8, "application/json");
        await _httpClient.PatchAsync(BaseUrl + "/Orders/" + order.Id, content);
    }

    public async Task DeleteOrder(string id)
    {
        await SetAuthHeader();
        await _httpClient.DeleteAsync(BaseUrl + "/Orders/" + id);
    }

    // ==================== User ====================

    private string UserToJson(AppUser user)
    {
        return JsonSerializer.Serialize(new
        {
            fields = new
            {
                Email     = new { stringValue    = user.Email },
                FullName  = new { stringValue    = user.FullName },
                Phone     = new { stringValue    = user.Phone },
                Role      = new { stringValue    = user.Role },
                CreatedAt = new { timestampValue = user.CreatedAt.ToString("O") },
                IsActive  = new { booleanValue   = user.IsActive }
            }
        });
    }

    private AppUser FirestoreToUser(FirestoreDocument doc)
    {
        Dictionary<string, FirestoreValue> f = doc.fields;

        AppUser user = new AppUser();
        user.Id        = GetDocId(doc);
        user.Email     = GetString(f, "Email");
        user.FullName  = GetString(f, "FullName");
        user.Phone     = GetString(f, "Phone");
        user.Role      = GetString(f, "Role");
        user.CreatedAt = GetTimestamp(f, "CreatedAt");
        user.IsActive  = GetBool(f, "IsActive");

        return user;
    }

    public async Task AddUser(AppUser user)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(UserToJson(user), Encoding.UTF8, "application/json");
        // PATCH with the Auth UID as the document ID
        await _httpClient.PatchAsync(BaseUrl + "/Users/" + user.Id, content);
    }

    public async Task<AppUser> GetUserById(string id)
    {
        await SetAuthHeader();

        var response = await _httpClient.GetAsync(BaseUrl + "/Users/" + id);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        FirestoreDocument doc = JsonSerializer.Deserialize<FirestoreDocument>(json, _jsonOptions);
        return FirestoreToUser(doc);
    }

    public async Task UpdateUser(AppUser user)
    {
        await SetAuthHeader();
        StringContent content = new StringContent(UserToJson(user), Encoding.UTF8, "application/json");
        await _httpClient.PatchAsync(BaseUrl + "/Users/" + user.Id, content);
    }
}

// ==================== Firestore response classes ====================
// These classes match the JSON structure that Firestore returns.
// JsonSerializer.Deserialize<T>() fills them in automatically.

class FirestoreList
{
    public List<FirestoreDocument> documents { get; set; } = new List<FirestoreDocument>();
}

class FirestoreDocument
{
    public string name { get; set; } = "";
    public Dictionary<string, FirestoreValue> fields { get; set; } = new Dictionary<string, FirestoreValue>();
}

// Each Firestore field has exactly one value type filled in, the rest are null
class FirestoreValue
{
    public string stringValue    { get; set; }
    public double? doubleValue   { get; set; }
    public string integerValue   { get; set; }
    public bool? booleanValue    { get; set; }
    public string timestampValue { get; set; }

    // For array fields (e.g. Order.Items)
    public FirestoreArrayValue arrayValue { get; set; }

    // For map fields (e.g. each item inside Order.Items)
    public FirestoreDocument mapValue { get; set; }
}

class FirestoreArrayValue
{
    public List<FirestoreValue> values { get; set; } = new List<FirestoreValue>();
}

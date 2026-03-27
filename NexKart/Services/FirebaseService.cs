using Firebase.Database;
using Firebase.Database.Query;
using NexKart.Models;

namespace NexKart.Services;

public class FirebaseService
{
    private readonly FirebaseClient _firebase =
        new("https://nexkart-65149-default-rtdb.firebaseio.com/");

    // -------------------- Product CRUD --------------------
    public async Task AddProduct(Product product)
    {
        await _firebase.Child("Products").PostAsync(product);
    }

    public async Task<List<Product>> GetProducts()
    {
        var items = await _firebase.Child("Products").OnceAsync<Product>();
        return items.Select(item => new Product
        {
            Id = item.Key,
            Name = item.Object.Name,
            Quantity = item.Object.Quantity,
            Price = item.Object.Price
        }).ToList();
    }

    public async Task<Product?> GetProductById(string id)
    {
        var product = await _firebase.Child("Products").Child(id).OnceSingleAsync<Product>();
        if (product is null) return null;
        product.Id = id;
        return product;
    }

    public async Task DeleteProduct(string id)
    {
        await _firebase.Child("Products").Child(id).DeleteAsync();
    }

    public async Task UpdateProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Id))
        {
            throw new ArgumentException("Product Id is required for update.");
        }

        await _firebase.Child("Products").Child(product.Id).PutAsync(product);
    }

    // -------------------- Order CRUD --------------------
    public async Task AddOrder(Order order)
    {
        await _firebase.Child("Orders").PostAsync(order);
    }

    public async Task<List<Order>> GetOrders()
    {
        var items = await _firebase.Child("Orders").OnceAsync<Order>();
        return items.Select(item =>
        {
            var order = item.Object;
            order.Id = item.Key;
            return order;
        }).ToList();
    }

    public async Task<Order?> GetOrderById(string id)
    {
        var order = await _firebase.Child("Orders").Child(id).OnceSingleAsync<Order>();
        if (order is null) return null;
        order.Id = id;
        return order;
    }

    public async Task UpdateOrder(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.Id))
        {
            throw new ArgumentException("Order Id is required for update.");
        }

        await _firebase.Child("Orders").Child(order.Id).PutAsync(order);
    }

    public async Task DeleteOrder(string id)
    {
        await _firebase.Child("Orders").Child(id).DeleteAsync();
    }

    // -------------------- User CRUD --------------------
    public async Task AddUser(AppUser user)
    {
        await _firebase.Child("Users").PostAsync(user);
    }

    public async Task<List<AppUser>> GetUsers()
    {
        var items = await _firebase.Child("Users").OnceAsync<AppUser>();
        return items.Select(item =>
        {
            var user = item.Object;
            user.Id = item.Key;
            return user;
        }).ToList();
    }

    public async Task<AppUser?> GetUserById(string id)
    {
        var user = await _firebase.Child("Users").Child(id).OnceSingleAsync<AppUser>();
        if (user is null) return null;
        user.Id = id;
        return user;
    }

    public async Task UpdateUser(AppUser user)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            throw new ArgumentException("User Id is required for update.");
        }

        await _firebase.Child("Users").Child(user.Id).PutAsync(user);
    }

    public async Task DeleteUser(string id)
    {
        await _firebase.Child("Users").Child(id).DeleteAsync();
    }
}

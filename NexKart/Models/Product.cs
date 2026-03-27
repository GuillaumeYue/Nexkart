namespace NexKart.Models;

public class Product
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}


using System.Text.Json.Serialization;

public class Book : BaseEntity
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public decimal Price { get; set; }
    public required string Genre { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }
    //birden fazla book saklamak ve hangi sepetlerde hangi siparişlerde hangi kitaplar var onları takip etmek için
    [JsonIgnore]
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>(); 

    [JsonIgnore]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}


using System.Text.Json.Serialization;

public class CartItem 
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int CartId { get; set; }
    public int BookId { get; set; }

    [JsonIgnore]
    public Book Book { get; set; } //hangş kitabı içerdiğini anlamak için
    [JsonIgnore]
    public virtual Cart Cart { get; set; } //carıtem hangi sepette onu bilmek için
}
using denemeData;
using Microsoft.EntityFrameworkCore;
public class OrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool PlaceOrder(int userId)
    {
        // Kullanıcının sepetini al
        var cart = _context.Carts.Include(c => c.Items)
                                 .ThenInclude(i => i.Book)
                                 .FirstOrDefault(c => c.UserId == userId);
        
        if (cart == null || !cart.Items.Any())
        {
            return false; // Sepet boşsa sipariş verilemez
        }

        // Toplam fiyatı hesapla
        decimal totalAmount = cart.Items.Sum(i => i.Book.Price * i.Quantity);

        // Kullanıcının bakiyesini kontrol et
        var user = _context.Users.Find(userId);
        if (user == null || user.Balance < totalAmount)
        {
            return false; // Bakiye yetersizse sipariş verilemez
        }

        // Kitap stoğunu kontrol et
        foreach (var item in cart.Items)
        {
            if (item.Book.Stock < item.Quantity)
            {
                return false; // Stoğu yetersiz olan kitap varsa sipariş verilemez
            }
        }

        // Sipariş oluştur
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            OrderItems = cart.Items.Select(i => new OrderItem
            {
                BookId = i.BookId,
                Quantity = i.Quantity,
                Price = i.Book.Price
            }).ToList()
        };

        _context.Orders.Add(order);

        // Kullanıcının bakiyesini güncelle
        user.Balance -= totalAmount;

        // Kitap stoklarını güncelle
        foreach (var item in cart.Items)
        {
            item.Book.Stock -= item.Quantity;
        }

        // Sepeti temizle
        _context.CartItems.RemoveRange(cart.Items);


        _context.SaveChanges();

        return true;
    }

    public List<Order> GetUserOrderHistory(int userId)
    {
        return _context.Orders
                       .Include(o => o.OrderItems)
                       .ThenInclude(oi => oi.Book)
                       .Where(o => o.UserId == userId)
                       .ToList();
    }
}

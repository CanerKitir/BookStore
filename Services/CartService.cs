using Microsoft.EntityFrameworkCore;
using denemeData;

public class CartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddToCart(int userId, int bookId, int quantity)
    {
        var user = _context.Users.Include(u => u.Cart)
                                 .ThenInclude(c => c.Items)
                                 .FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var cart = user.Cart ?? new Cart { UserId = userId };
        var cartItem = cart.Items.FirstOrDefault(ci => ci.BookId == bookId);

        if (cartItem == null)
        {
            cartItem = new CartItem { BookId = bookId, Quantity = quantity, Cart = cart };
            cart.Items.Add(cartItem);
        }
        else
        {
            cartItem.Quantity += quantity;
        }

        if (cart.Id == 0)
        {
            _context.Carts.Add(cart);
        }
        else
        {
            _context.Carts.Update(cart);
        }

        _context.SaveChanges();
    }

    public List<CartItem> GetCartItems(int userId)
    {
        var user = _context.Users.Include(u => u.Cart)
                                 .ThenInclude(c => c.Items)
                                 .ThenInclude(ci => ci.Book)
                                 .FirstOrDefault(u => u.Id == userId);
        if (user == null || user.Cart == null)
        {
            return new List<CartItem>();
        }

        return (List<CartItem>)user.Cart.Items;
    }
    public List<Cart> GetAllCarts()
    {
        return _context.Carts.Include(c => c.Items)
                             .ThenInclude(ci => ci.Book)
                             .ToList();
    }
    public void RemoveFromCart(int userId, int bookId, int quantity)
    {
        var user = _context.Users.Include(u => u.Cart)
                                 .ThenInclude(c => c.Items)
                                 .FirstOrDefault(u => u.Id == userId);
        if (user == null || user.Cart == null)
        {
            throw new Exception("User or cart not found");
        }

        var cartItem = user.Cart.Items.FirstOrDefault(ci => ci.BookId == bookId);
        if (cartItem == null)
        {
            throw new Exception("Item not found in cart");
        }

        cartItem.Quantity -= quantity;
        if (cartItem.Quantity <= 0)
        {
            user.Cart.Items.Remove(cartItem);
            _context.CartItems.Remove(cartItem);
        }

        _context.SaveChanges();
    }

    public void DeleteFromCart(int userId, int bookId)
    {
        var user = _context.Users.Include(u => u.Cart)
                                 .ThenInclude(c => c.Items)
                                 .FirstOrDefault(u => u.Id == userId);
        if (user == null || user.Cart == null)
        {
            throw new Exception("User or cart not found");
        }

        var cartItem = user.Cart.Items.FirstOrDefault(ci => ci.BookId == bookId);
        if (cartItem == null)
        {
            throw new Exception("Item not found in cart");
        }

        user.Cart.Items.Remove(cartItem);
        _context.CartItems.Remove(cartItem);

        _context.SaveChanges();
    }
}

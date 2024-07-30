using Microsoft.EntityFrameworkCore;

namespace denemeData
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Order> Orders { get; internal set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User ve Cart arasında bire bir ilişki
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            // Cart ve CartItem arasında bire çok ilişki
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);

            // CartItem ve Book arasında bire çok ilişki
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BookId);

            // Order ve User arasında bire çok ilişki
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Order ve OrderItem arasında bire çok ilişki
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem ve Book arasında bire çok ilişki
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(oi => oi.BookId);

            modelBuilder.Entity<User>() // bakiyenin 2 haneli olmasını sağlama
                .Property(u => u.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Book>() // ücretin 2 haneli olmasını sağlama
                .Property(u => u.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>() // ücretin 2 haneli olmasını sağlama
                .Property(u => u.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>() // ücretin 2 haneli olmasını sağlama
                .Property(u => u.Price)
                .HasPrecision(18, 2);
        }

    }
}

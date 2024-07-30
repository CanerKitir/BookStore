using denemeData;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace denemeBookService
{
    public class BookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(int id)
        {
            return _context.Books.Find(id);
        }

        public void AddBook(Book book)
        {
            Book newbook = new Book
            {
                Title=book.Title,
                Author=book.Author,
                Price = book.Price,
                Genre = book.Genre,
                Description = book.Description,
                Stock = book.Stock,
                UpdateTime = DateTime.Now,
                CreatedTime = DateTime.Now
            };
            _context.Books.Add(newbook);
            _context.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            var existingBook = _context.Books.Local.FirstOrDefault(u => u.Id == book.Id);
            if (existingBook != null)
            {
                _context.Entry(existingBook).State = EntityState.Detached;
            }
            book.UpdateTime = DateTime.Now;
            _context.Books.Update(book);
            _context.SaveChanges();
        }

        public void DeleteBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
        }

        public List<Book> GetBooksByGenre(string genre)
        {
            return _context.Books
                .Where(b => b.Genre.ToLower() == genre.ToLower())
                .ToList();
        }
    }
}

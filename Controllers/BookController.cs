using Microsoft.AspNetCore.Mvc;
using denemeBookService;
using Microsoft.AspNetCore.Authorization;

namespace denemeBookController
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Book>> Get()
        {
            return _bookService.GetAllBooks();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<Book> Get(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        [HttpGet("genre/{genre}")]
        [AllowAnonymous]
        public ActionResult<List<Book>> GetBooksByGenre(string genre)
        {
            var books = _bookService.GetBooksByGenre(genre);
            if (books == null || books.Count == 0)
            {
                return NotFound("No books found for the specified genre.");
            }
            return Ok(books);
        }

        [HttpPost("/Add/Book")]
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult Post([FromBody] Book book)
        {
            _bookService.AddBook(book);
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [HttpPut("/Update/Book")]
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult UpdateBook([FromBody] Book book)
        {
            Book existingBook = _bookService.GetBookById(book.Id);
            if (existingBook == null)
            {
                return NotFound();
            }

            _bookService.UpdateBook(book);
            return Ok("Book Updated");
        }

        [HttpDelete("/Delete/Book/{id}")]
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            _bookService.DeleteBook(id);
            return Ok("Book Deleted");
        }
    }
}

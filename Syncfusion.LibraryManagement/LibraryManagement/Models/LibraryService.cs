using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
	public class LibraryService : ILibraryService
    {
        private LibraryContext _context;
        public LibraryService(LibraryContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public void DeleteBook(long id)
        {
	        Book ord = _context.Books.Find(id);
	        _context.Books.Remove(ord);
	        _context.SaveChanges();
        }

        public IQueryable<Book> GetBooks()
        {
	        return _context.Books;
        }

        public void InsertBook(Book book)
        {
	        _context.Books.Add(book);
	        _context.SaveChanges();
        }

        public Book SingleBook(long id)
        {
            throw new NotImplementedException();
        }

        public void UpdateBook(long id, Book book)
        {
	        var local = _context.Set<Book>().Local.FirstOrDefault(entry => entry.Id.Equals(book.Id));
	        // check if local is not null
	        if (local != null)
	        {
		        // detach
		        _context.Entry(local).State = EntityState.Detached;
	        }
	        _context.Entry(book).State = EntityState.Modified;
	        _context.SaveChanges();
        }
    }
}

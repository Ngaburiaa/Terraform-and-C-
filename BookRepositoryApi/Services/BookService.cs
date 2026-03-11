using BookRepositoryApi.Data;
using BookRepositoryApi.Models;
using BookRepositoryApi.Services.Interfaces;

namespace BookRepositoryApi.Services;

public sealed class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public IReadOnlyCollection<Book> GetAll()
    {
        return _context.Books
            .OrderBy(b => b.Title)
            .ToList();
    }

    public Book? GetById(int id)
    {
        return _context.Books.FirstOrDefault(b => b.Id == id);
    }

    public Book Create(CreateBookRequest request)
    {
        var book = new Book
        {
            // let the database generate the id
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            Isbn = request.Isbn.Trim(),
            YearPublished = request.YearPublished
        };

        _context.Books.Add(book);
        _context.SaveChanges();

        return book;
    }

    public bool Update(int id, UpdateBookRequest request)
    {
        var book = _context.Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return false;

        book.Title = request.Title.Trim();
        book.Author = request.Author.Trim();
        book.Isbn = request.Isbn.Trim();
        book.YearPublished = request.YearPublished;

        _context.SaveChanges();

        return true;
    }

    public bool Delete(int id)
    {
        var book = _context.Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return false;

        _context.Books.Remove(book);
        _context.SaveChanges();

        return true;
    }
}

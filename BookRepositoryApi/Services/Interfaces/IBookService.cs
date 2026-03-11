namespace BookRepositoryApi.Services.Interfaces;

public interface IBookService
{
    IReadOnlyCollection<BookRepositoryApi.Models.Book> GetAll();
    BookRepositoryApi.Models.Book? GetById(int id);
    BookRepositoryApi.Models.Book Create(BookRepositoryApi.Models.CreateBookRequest request);
    bool Update(int id, BookRepositoryApi.Models.UpdateBookRequest request);
    bool Delete(int id);
}

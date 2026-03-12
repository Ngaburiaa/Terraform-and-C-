using BookRepositoryApi.Models;

namespace BookRepositoryApi.Services.Interfaces;

public interface IBookService
{
    IReadOnlyCollection<BookResponse> GetAll();
    BookResponse? GetById(int id);
    BookResponse Create(CreateBookRequest request, int authorId);
    bool Update(int id, UpdateBookRequest request);
    bool Delete(int id);
}

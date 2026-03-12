using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

public interface IUserService
{
    IReadOnlyCollection<UserResponse> GetAll();
    UserResponse? GetById(int id);
    bool Delete(int id); // allow admin to remove a user (books cascade)
}

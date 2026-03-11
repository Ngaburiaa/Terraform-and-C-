using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

public interface IUserService
{
    IReadOnlyCollection<UserResponse> GetAll();
    UserResponse? GetById(int id);
}

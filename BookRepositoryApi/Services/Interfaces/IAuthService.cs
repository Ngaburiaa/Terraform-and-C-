using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

public interface IAuthService
{
    LoginResponse? Login(LoginRequest request);
    LoginResponse? Register(RegisterRequest request);
}

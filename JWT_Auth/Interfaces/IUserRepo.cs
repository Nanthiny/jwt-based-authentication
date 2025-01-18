using JWT_Auth.Dtos;
using JWT_Auth.Models;

namespace JWT_Auth.Interfaces
{
	public interface IUserRepo
	{
		Task<User> RegisterAsync(RegisterRequest user);
		Task<AuthResponse> LoginAsync(User user);
		Task<User> GetUserExistAsync(LoginRequest user);
		Task<User> GetUserByEmailExistAsync(string email);
		Task<string> GenerateToken(User user);
		string GenerateRefreshToken();
	
	}
}

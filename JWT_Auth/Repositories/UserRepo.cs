using JWT_Auth.DbContexts;
using JWT_Auth.Dtos;
using JWT_Auth.Interfaces;
using JWT_Auth.Models;
using JWT_Auth.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Auth.Repositories
{
	public class UserRepo : IUserRepo
	{
		private readonly SystemDbContext _context;
		private readonly TokenService _service;
		private readonly IConfiguration _configuration;
		public UserRepo(SystemDbContext context,IConfiguration configuration, TokenService service)
		{
				_context=context;
			_configuration = configuration;
			_service = service;
		}
		//find existing user based on credentials
		public async Task<User> GetUserExistAsync(LoginRequest user)
		{
			//user that matches the email and password provided.
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
		}

		public async Task<AuthResponse> LoginAsync(User user)
		{
			//get generated refresh token value
			var refreshToken = GenerateRefreshToken();
			//create a new AuthResponse object to store the user's email, username, token, and role.
			var authUser = new AuthResponse
			{
				Email = user.Email,
				UserName = user.Username,
				Token = await GenerateToken(user),
				Role = user.RoleId==null ? null: (await GetRoleById(user.RoleId)),
				RefreshToken= refreshToken
			};
			//save the refresh token to the database.
			await _service.SaveRefreshToken(user.Email, refreshToken);
			return authUser;
		}
		//get user role name by Id
		private async Task<string> GetRoleById(int? roleId)
		{
			return (await _context.Roles.FindAsync(roleId)).Name;
		}
		public async Task<User> RegisterAsync(RegisterRequest user)
		{
			var userObj= new User
			{
				Email = user.Email,
				Password = user.Password,
				Username = user.Username
			};
			await _context.Users.AddAsync(userObj);
			await _context.SaveChangesAsync();
			return userObj;
		}
		public async Task<string> GenerateToken(User user)
		{
			// Creates a new symmetric security key from the JWT key specified in the app configuration.
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			// Sets up the signing credentials using the above security key and specifying the HMAC SHA256 algorithm.
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			// Defines a set of claims to be included in the token.
			var claims = new List<Claim>
			{
                // Custom claim using the user's ID.
                new Claim("Myapp_User_Id", user.UserId.ToString()),
                // Standard claim for user identifier, using username.
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                // Standard claim for user's email.
                new Claim(ClaimTypes.Email, user.Email),
                // Standard JWT claim for subject, using user ID.
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString())
			};
			// Adds a role claim for each role associated with the user.
			if(user.RoleId != null)
				claims.Add(new Claim(ClaimTypes.Role, (await GetRoleById(user.RoleId))));
			
			// Creates a new JWT token with specified parameters including issuer, audience, claims, expiration time, and signing credentials.
			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(1), // Token expiration set to 1 hour from the current time.
				signingCredentials: credentials);
			// Serializes the JWT token to a string and returns it.
			return new JwtSecurityTokenHandler().WriteToken(token);
			
		}
		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];  // Prepare a buffer to hold the random bytes.
			using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);  // Fill the buffer with cryptographically strong random bytes.
				return Convert.ToBase64String(randomNumber);  // Convert the bytes to a Base64 string and return.
			}
			
		}

		public async Task<User> GetUserByEmailExistAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
		}
	
	}
}

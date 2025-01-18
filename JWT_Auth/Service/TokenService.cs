using JWT_Auth.DbContexts;
using JWT_Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT_Auth.Service
{
	public class TokenService
	{
		private readonly SystemDbContext _context;
		public TokenService(SystemDbContext context)
		{
				_context=context;
		}
		public async Task SaveRefreshToken(string email, string token)
		{
			// Create a new RefreshToken object.
			var refreshToken = new RefreshToken
			{
				Email = email,  // Set the email associated with the token.
				Token = token,  // Set the token value.
				ExpiryDate = DateTime.Today.AddDays(3)  // Set the expiration date to 3 days from now.
			};
			
			_context.RefreshTokens.Add(refreshToken);			
			await _context.SaveChangesAsync();
		}
		public async Task<string> RetrieveUserEmailByRefreshToken(string refreshToken)
		{
			// Asynchronously find a refresh token that matches the provided token and has not yet expired.
			var tokenRecord = await _context.RefreshTokens
				.FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow);
			
			return tokenRecord?.Email;
		}
		public async Task<bool> RevokeRefreshToken(string refreshToken)
		{

			var tokenRecord = await _context.RefreshTokens
				.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
			// If the token is found, remove it from the DbSet.
			if (tokenRecord != null)
			{
				_context.RefreshTokens.Remove(tokenRecord);
				await _context.SaveChangesAsync();
				return true;
			}

			return false;
		}
	}
}

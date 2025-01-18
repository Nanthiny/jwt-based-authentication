namespace JWT_Auth.Dtos
{
	public class AuthResponse
	{
		public required string UserName { get; set; }
		public required string Email { get; set; }
		public string? Role { get; set; }
		public required string Token { get; set; }
		public required string RefreshToken { get; set; }
	}
}

namespace JWT_Auth.Dtos
{
	public class AuthResponse
	{
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Role { get; set; }
		public string? Token { get; set; }
	}
}

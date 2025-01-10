using System.ComponentModel.DataAnnotations;

namespace JWT_Auth.Dtos
{
	public class LoginRequest
	{
		[EmailAddress]
		public required string Email { get; set; }
		[StringLength(15, MinimumLength = 8, ErrorMessage = "Password should contains characters length between 8 and 15...")]
		public required string Password { get; set; }
	}
}

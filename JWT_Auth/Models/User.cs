using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Auth.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }

		public required string Username { get; set; }
		[StringLength(15, MinimumLength = 8, ErrorMessage = "Password should contains characters length between 8 and 15...")]

		public required string Password { get; set; }
		[EmailAddress]
		public required string Email { get; set; }
		[ForeignKey("RoleId")]
		public int? RoleId { get; set; }
		public Role? Role { get; set; }
	}
}

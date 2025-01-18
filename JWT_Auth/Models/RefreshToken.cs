using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT_Auth.Models
{
	public class RefreshToken
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public required string Token { get; set; }
		public required string Email { get; set; }
		public required DateTime ExpiryDate { get; set; }
	}
}

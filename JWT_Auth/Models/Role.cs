using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JWT_Auth.Models
{
	public class Role
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RoleId { get; set; }
		public required string Name { get; set; }	
		public List<User>? Users { get; set; }
	}
}

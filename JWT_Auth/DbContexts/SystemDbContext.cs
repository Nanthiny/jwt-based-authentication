using JWT_Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT_Auth.DbContexts
{
	public class SystemDbContext:DbContext
	{
		public SystemDbContext(DbContextOptions<SystemDbContext> options) : base(options)
		{

		}
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
	
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{			
			modelBuilder.Entity<Role>()				
				.HasMany(u => u.Users)
				.WithOne(ur => ur.Role)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();			
		}
	}
}

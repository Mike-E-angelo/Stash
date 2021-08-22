using Microsoft.EntityFrameworkCore;

namespace EfCore.ScopedVsTransaction
{
	public sealed class Context : DbContext
	{
		public Context(DbContextOptions options) : base(options) {}

		public DbSet<Subject> Subjects { get; set; } = default!;
	}
}
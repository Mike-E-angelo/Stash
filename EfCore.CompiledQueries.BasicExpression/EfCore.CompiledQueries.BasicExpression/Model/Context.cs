using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	sealed class Context : DbContext
	{
		public Context(DbContextOptions options) : base(options) {}

		public DbSet<Subject> Subjects { get; set; } = default!;
	}
}
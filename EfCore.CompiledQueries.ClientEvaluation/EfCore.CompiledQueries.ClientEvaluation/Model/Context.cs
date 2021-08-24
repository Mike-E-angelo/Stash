using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	sealed class Context : DbContext
	{
		public Context(DbContextOptions options) : base(options) {}

		public DbSet<Subject> Subjects { get; set; } = default!;
	}
}
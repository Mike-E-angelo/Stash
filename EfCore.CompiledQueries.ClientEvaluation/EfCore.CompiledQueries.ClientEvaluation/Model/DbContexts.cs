using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public sealed class DbContexts<T> : IContexts<T> where T : DbContext
	{
		readonly IDbContextFactory<T> _factory;

		public DbContexts(IDbContextFactory<T> factory) => _factory = factory;

		public T Get() => _factory.CreateDbContext();
	}
}
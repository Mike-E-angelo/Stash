using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public class SqlDbContexts<T> : IDbContextFactory<T> where T : DbContext
	{
		readonly IDbContextFactory<T> _factory;

		public SqlDbContexts(string name)
			: this(new DbContextOptionsBuilder<T>().UseSqlServer(name).Options) {}

		public SqlDbContexts(DbContextOptions<T> options) : this(new PooledDbContextFactory<T>(options)) {}

		public SqlDbContexts(IDbContextFactory<T> factory) => _factory = factory;

		public T CreateDbContext() => _factory.CreateDbContext();

		public Task<T> CreateDbContextAsync(CancellationToken cancellationToken = new CancellationToken())
			=> _factory.CreateDbContextAsync();
	}
}
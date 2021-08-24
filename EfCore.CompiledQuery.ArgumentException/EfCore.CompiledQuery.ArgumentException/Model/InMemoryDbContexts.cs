using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public class InMemoryDbContexts<T> : IDbContextFactory<T> where T : DbContext
	{
		readonly IDbContextFactory<T> _factory;

		public InMemoryDbContexts() : this(Guid.NewGuid().ToString()) {}

		public InMemoryDbContexts(string name)
			: this(new DbContextOptionsBuilder<T>().UseInMemoryDatabase(name).Options) {}

		public InMemoryDbContexts(DbContextOptions<T> options) : this(new PooledDbContextFactory<T>(options)) {}

		public InMemoryDbContexts(IDbContextFactory<T> factory) => _factory = factory;

		public T CreateDbContext() => _factory.CreateDbContext();
	}
}
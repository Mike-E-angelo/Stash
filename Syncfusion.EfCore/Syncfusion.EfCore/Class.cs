using Microsoft.EntityFrameworkCore;

namespace Syncfusion.EfCore
{
	public class SimpleContext : DbContext
	{
		public SimpleContext(DbContextOptions<SimpleContext> options) : base(options) {}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Simple>()
			            .HasData(new Simple { Id = 1, Name = "Hello" }, new Simple { Id = 2, Name = "World" });
		}

		public DbSet<Simple> Items { get; set; }
	}

	public class Simple
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
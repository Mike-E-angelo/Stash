using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Linq;
using System.Threading.Tasks;

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

	// Implementing custom adaptor by extending the DataAdaptor class
	public class CustomAdaptor : DataAdaptor
	{
		readonly IQueryable<Simple> _query;

		public CustomAdaptor(SimpleContext context) : this(context.Items) {}

		CustomAdaptor(IQueryable<Simple> context) => _query = context;

		public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
		{
			var query = _query;

			if (dm.Search?.Count > 0)
			{
				query = DataOperations.PerformSearching(query, dm.Search);
			}

			if (dm.Sorted?.Count > 0)
			{
				// Sorting
				query = DataOperations.PerformSorting(query, dm.Sorted);
			}

			if (dm.Where?.Count > 0)
			{
				// Filtering
				query = DataOperations.PerformFiltering(query, dm.Where, dm.Where[0].Operator);
			}

			if (dm.Skip != 0)
			{
				//Paging
				query = DataOperations.PerformSkip(query, dm.Skip);
			}

			if (dm.Take != 0)
			{
				query = DataOperations.PerformTake(query, dm.Take);
			}

			var result = dm.RequiresCounts
				             ? new DataResult { Result = await query.ToListAsync(), Count = await query.CountAsync() }
				             : (object)query;
			return result;
		}
	}
}
using EFGrid.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EFGrid.Shared.DataAccess
{
	public class OrderContext : DbContext
	{
		public virtual DbSet<Order> Orders { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder
				.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=TestGridAPI;Trusted_Connection=True")
				.EnableDetailedErrors()
				.EnableSensitiveDataLogging()
				.LogTo(x => Debug.WriteLine(x))
				;
		}
	}
}
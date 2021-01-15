using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EfCore.Interpolation
{
	public class UnitTest1
	{
		[Fact]
		public async Task Works()
		{
			{
				var context = new Context();
				await context.Database.EnsureCreatedAsync();

				if (!await context.Users.AnyAsync())
				{
					var entity = new User();
					entity.Firsts.Add(new First());
					entity.Seconds.Add(new Second { Other = new Other {Name =  "Hello World"}});

					context.Users.Add(entity);
					await context.SaveChangesAsync();
				}
			}

			{
				var context = new Context();
				var query   = context.Users;
				var first = query.SelectMany(x => x.Firsts).Select(x => new
				{
					Name     = nameof(First),
					Property = x.Id.ToString()
				});
				var second = query.SelectMany(x => x.Seconds).Select(x => new
				{
					Temp = x.Other.Name
				}).Select(x => new
				{
					Name     = nameof(First),
					Property = x.Temp + " Interpolated"
				});

				var all = await first.Concat(second).ToArrayAsync();

				Assert.NotEmpty(all);
			}


		}


		[Fact]
		public async Task DoesntWork()
		{
			{
				var context = new Context();
				await context.Database.EnsureCreatedAsync();

				if (!await context.Users.AnyAsync())
				{
					var entity = new User();
					entity.Firsts.Add(new First());
					entity.Seconds.Add(new Second { Other = new Other {Name =  "Hello World"}});

					context.Users.Add(entity);
					await context.SaveChangesAsync();
				}
			}

			{
				var context = new Context();
				var query   = context.Users;
				var first   = query.SelectMany(x => x.Firsts).Select(x => new
				{
					Name = nameof(First),
					Property = x.Id.ToString()
				});
				var second = query.SelectMany(x => x.Seconds).Select(x => new
				{
					Temp = x.Other.Name
				}).Select(x => new
				{
					Name     = nameof(First),
					Property = $"{x.Temp} Interpolated"
				});

				var all = await first.Concat(second).ToArrayAsync();

				Assert.NotEmpty(all);
			}


		}


		public class Context : DbContext
		{
			public Context() : this(new DbContextOptionsBuilder<Context>()
			                        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=efcorelocaltesting;Trusted_Connection=True;MultipleActiveResultSets=true")
			                        .Options) {}

			public Context(DbContextOptions<Context> options) : base(options) {}

			protected override void OnModelCreating(ModelBuilder modelBuilder) {}

			public DbSet<User> Users { get; set; }

			public DbSet<Common> Commons { get; set; }
		}

		public class User
		{
			public virtual Guid Id { get; set; }

			public virtual ICollection<First> Firsts { get; set; } = new List<First>();
			public virtual ICollection<Second> Seconds { get; set; } = new List<Second>();
		}

		public class Common
		{
			public virtual Guid Id { get; set; }
		}

		public class First : Common {}

		public class Second : Common
		{
			public virtual Other Other { get; set; }
		}

		public class Other
		{
			public virtual Guid Id { get; set; }

			public virtual string Name { get; set; }
		}
	}
}
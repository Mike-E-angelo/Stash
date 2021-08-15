using DragonSpark.Application.Entities;
using DragonSpark.Model.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfCore6.Model.Entities
{
	public class ApplicationState : Storage<User>
	{
		public ApplicationState(DbContextOptions<ApplicationState> options)
			: this(options, DefaultStorageInitializer.Default) {}

		[ActivatorUtilitiesConstructor]
		public ApplicationState(DbContextOptions<ApplicationState> options, IStorageInitializer initializer)
			: base(options, initializer, EmptyCommand<ModelBuilder>.Default) {}
	}
}
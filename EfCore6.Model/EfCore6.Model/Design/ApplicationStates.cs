using DragonSpark.Application.Entities;
using DragonSpark.Model.Selection;
using EfCore6.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCore6.Model.Design
{
	sealed class ApplicationStates : ISelect<DbContextOptions<ApplicationState>, ApplicationState>
	{
		public static ApplicationStates Default { get; } = new ApplicationStates();

		ApplicationStates() : this(KnownStorageInitializers.Default) {}

		readonly IStorageInitializer _initializer;

		public ApplicationStates(IStorageInitializer initializer) => _initializer = initializer;

		public ApplicationState Get(DbContextOptions<ApplicationState> parameter) => new(parameter, _initializer);
	}
}
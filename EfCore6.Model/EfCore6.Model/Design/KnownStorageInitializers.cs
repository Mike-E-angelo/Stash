using DragonSpark.Application.Entities;

namespace EfCore6.Model.Design
{
	sealed class KnownStorageInitializers : StorageInitializer
	{
		public static KnownStorageInitializers Default { get; } = new KnownStorageInitializers();

		KnownStorageInitializers() {}
	}
}
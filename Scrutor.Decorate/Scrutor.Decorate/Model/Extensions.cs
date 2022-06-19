using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public static class Extensions
{
	public static IOperation<T> Identified<T>(this IOperation<T> @this, Guid identifier) where T : ExternalProcess
		=> new IdentifiedProcessOperation<T>(@this, identifier);

	public static StartingProcess<T> Process<T>(this ModelContext _, Saving<T> saving) where T : ExternalProcess
		=> new(saving);
}
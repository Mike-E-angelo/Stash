using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class HandledComplete<T> : Select<StartSubsequentStep<T>, IOperation<T>>, IComplete<T>
	where T : ExternalProcess
{
	public static HandledComplete<T> Default { get; } = new();

	HandledComplete() : base(x => x.Completing.With.Default.And.Handling.With.Default) {}
}
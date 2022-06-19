using DragonSpark.Model.Operations;
using DragonSpark.Model.Selection;

namespace Scrutor.Decorate.Model;

public class Operation<T> : Select<T, ValueTask>, IOperation<T>
{
	public Operation(ISelect<T, ValueTask> select) : this(select.Get) {}

	public Operation(Func<T, ValueTask> select) : base(select) {}
}
using DragonSpark.Compose;
using DragonSpark.Model.Operations;
using DragonSpark.Model.Selection.Conditions;

namespace Scrutor.Decorate.Model;

sealed class Handle<T> : IOperation<T> where T : ExternalProcess
{
	readonly IOperation<T>         _previous;
	readonly IStatus               _status;
	readonly ICondition<Exception> _avoid;

	public Handle(IOperation<T> previous, IStatus status) : this(previous, status, ShouldAvoidStatus.Default) {}

	public Handle(IOperation<T> previous, IStatus status, ICondition<Exception> avoid)
	{
		_previous = previous;
		_status   = status;
		_avoid    = avoid;
	}

	public async ValueTask Get(T parameter)
	{
		try
		{
			await _previous.Await(parameter);
		}
		catch (Exception err) when (_avoid.Get(err))
		{
			throw;
		}
		catch
		{
			await _status.Await(parameter);
			throw;
		}
	}
}
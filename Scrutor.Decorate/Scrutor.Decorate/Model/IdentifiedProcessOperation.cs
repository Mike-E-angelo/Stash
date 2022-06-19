using DragonSpark.Compose;
using DragonSpark.Model.Operations;
using NetFabric.Hyperlinq;

namespace Scrutor.Decorate.Model;

sealed class IdentifiedProcessOperation<T> : IOperation<T> where T : ExternalProcess
{
	readonly IOperation<T> _previous;
	readonly Guid          _identifier;

	public IdentifiedProcessOperation(IOperation<T> previous, Guid identifier)
	{
		_previous   = previous;
		_identifier = identifier;
	}

	public async ValueTask Get(T parameter)
	{
		foreach (var step in parameter.CompletedSteps.AsValueEnumerable())
		{
			if (step.Identifier == _identifier)
			{
				return;
			}
		}

		await _previous.Await(parameter);
		parameter.CompletedSteps.Add(new CompletedStep { Identifier = _identifier });
	}
}
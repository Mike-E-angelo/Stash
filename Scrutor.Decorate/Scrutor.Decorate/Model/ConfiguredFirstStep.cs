using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public class ConfiguredFirstStep<T> : IFirstStep<T> where T : ExternalProcess
{
	readonly IOperation<T> _operation;
	readonly string        _message;

	protected ConfiguredFirstStep(IOperation<T> operation, string message)
	{
		_operation = operation;
		_message   = message;
	}

	public StartSubsequentStep<T> Get(Saving<T> parameter) => Start.A.Process(parameter)
	                                                               .Starting.With.Message(_message)
	                                                               .Add(_operation);
}
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model.Compose;


public class Step<T> : IStep<T> where T : ExternalProcess
{
	readonly IOperation<T> _operation;
	readonly string        _message;

	public Step(IOperation<T> operation, string message)
	{
		_operation = operation;
		_message   = message;
	}

	public StartSubsequentStep<T> Get(StartSubsequentStep<T> parameter)
		=> parameter.Then.Add(_operation).With.Message(_message);
}
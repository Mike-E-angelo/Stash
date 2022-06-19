using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class SubsequentStepWithMessage<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _current;
	readonly IOperation<T> _step;

	public SubsequentStepWithMessage(Saving<T> saving, IOperation<T> current, IOperation<T> step)
	{
		_saving  = saving;
		_current = current;
		_step    = step;
	}

	public StartSubsequentStep<T> None => Step(ProcessingDefinition.Default);

	public StartSubsequentStep<T> Message(string message) => Step(new ProcessingDefinition(message));

	StartSubsequentStep<T> Step(StatusDefinition definition)
		=> new(_saving, _current.Then().Append(Message(definition)).Out());

	IOperation<T> Message(StatusDefinition definition) => Start.A.Selection<T>()
	                                                           .By.CastDown<ExternalProcess>()
	                                                           .Select(definition.Get(_saving.Process))
	                                                           .Then()
	                                                           .Append(_step)
	                                                           .Append(_saving.Save)
	                                                           .Out();
}
using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class StartingMessage<T> where T : ExternalProcess
{
	readonly Saving<T> _saving;

	public StartingMessage(Saving<T> saving) => _saving = saving;

	public Step<T> None => Step(ProcessingDefinition.Default);

	public Step<T> Message(string message) => Step(new ProcessingDefinition(message));

	Step<T> Step(StatusDefinition definition) => new(_saving, Message(definition));

	IOperation<T> Message(StatusDefinition definition) => Start.A.Selection<T>()
	                                                           .By.CastDown<ExternalProcess>()
	                                                           .Select(definition.Get(_saving.Process))
	                                                           .Then()
	                                                           .Out();
}
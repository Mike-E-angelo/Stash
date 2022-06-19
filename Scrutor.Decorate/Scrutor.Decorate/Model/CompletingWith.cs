using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class CompletingWith<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public CompletingWith(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public AndContext<T> Default => Message("Done!");

	public AndContext<T> Message(string message) => Step(new CompletedDefinition(message));

	AndContext<T> Step(StatusDefinition definition)
		=> new(_saving, _subject.Then().Append(Message(definition)).Out());

	IOperation<T> Message(StatusDefinition definition) => Start.A.Selection<T>()
	                                                           .By.CastDown<ExternalProcess>()
	                                                           .Select(definition.Get(_saving.Process))
	                                                           .Then()
	                                                           .Out();
}
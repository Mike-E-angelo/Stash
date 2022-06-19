using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class HandlingWith<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public HandlingWith(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public IOperation<T> Default => Step(ErrorDefinition.Default);

	public IOperation<T> Message(string message) => Step(new ErrorDefinition(message));

	IOperation<T> Step(StatusDefinition definition) => new Handle<T>(_subject, Message(definition));

	IStatus Message(StatusDefinition definition) => definition.Get(_saving.Process);
}
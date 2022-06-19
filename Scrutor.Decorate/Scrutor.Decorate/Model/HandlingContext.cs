using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class HandlingContext<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public HandlingContext(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public HandlingWithContext<T> Handling => new(_saving, _subject);
}
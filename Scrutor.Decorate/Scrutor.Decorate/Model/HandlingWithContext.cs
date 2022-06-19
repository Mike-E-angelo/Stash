using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class HandlingWithContext<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public HandlingWithContext(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public HandlingWith<T> With => new(_saving, _subject);
}
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class Completing<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public Completing(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public CompletingWith<T> With => new(_saving, _subject);
}
using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class AndContext<T> : CompleteContext<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public AndContext(Saving<T> saving, IOperation<T> subject) : base(subject.Await)
	{
		_saving  = saving;
		_subject = subject;
	}

	public HandlingContext<T> And => new(_saving, _subject);
}
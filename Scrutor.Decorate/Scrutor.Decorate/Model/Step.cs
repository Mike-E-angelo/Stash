using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class Step<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public Step(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public StartSubsequentStep<T> Add(IOperation<T> subject)
		=> new(_saving, _subject.Then().Append(subject.Then().Append(_saving.Save)).Out());

	public SubjectStep<T, TSubject> Add<TSubject>(IOperation<TSubject> subject)
		=> new(_saving, _subject, subject);
}
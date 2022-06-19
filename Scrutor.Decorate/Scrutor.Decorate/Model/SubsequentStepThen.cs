using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class SubsequentStepThen<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public SubsequentStepThen(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public SubsequentStepWith<T> Add(IOperation<T> subject)
		=> new(_saving, _subject, subject);

	public SubsequentStep<T, TSubject> Add<TSubject>(IOperation<TSubject> subject)
		=> new(_saving, _subject, subject);
}
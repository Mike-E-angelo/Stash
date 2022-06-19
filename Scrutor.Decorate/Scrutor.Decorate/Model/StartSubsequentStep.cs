using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class StartSubsequentStep<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _subject;

	public StartSubsequentStep(Saving<T> saving, IOperation<T> subject)
	{
		_saving  = saving;
		_subject = subject;
	}

	public SubsequentStepThen<T> Then => new(_saving, _subject);

	public Completing<T> Completing => new(_saving, _subject);
}
using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class SubsequentStep<T, TSubject> where T : ExternalProcess
{
	readonly Saving<T>            _saving;
	readonly IOperation<T>        _current;
	readonly IOperation<TSubject> _subject;

	public SubsequentStep(Saving<T> saving, IOperation<T> current, IOperation<TSubject> subject)
	{
		_saving  = saving;
		_current = current;
		_subject = subject;
	}

	public SubsequentStepWith<T> Using(Func<T, TSubject> subject)
		=> new(_saving, _current, Start.A.Selection<T>()
		                               .By.Calling(subject)
		                               .Select(_subject)
		                               .Then()
		                               .Append(_saving.Save)
		                               .Out());
}
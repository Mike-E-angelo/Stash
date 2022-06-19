using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

public sealed class SubsequentStepWith<T> where T : ExternalProcess
{
	readonly Saving<T>     _saving;
	readonly IOperation<T> _current;
	readonly IOperation<T> _step;

	public SubsequentStepWith(Saving<T> saving, IOperation<T> current, IOperation<T> step)
	{
		_saving  = saving;
		_current = current;
		_step    = step;
	}

	public SubsequentStepWithMessage<T> With => new(_saving, _current, _step);
}
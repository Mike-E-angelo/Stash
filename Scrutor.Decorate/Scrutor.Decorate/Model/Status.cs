using DragonSpark.Compose;
using DragonSpark.Model.Operations;

namespace Scrutor.Decorate.Model;

sealed class Status : IStatus
{
	readonly IUpdate                     _update;
	readonly IOperation<ExternalProcess> _updater;

	public Status(IUpdate update, IOperation<ExternalProcess> updater)
	{
		_updater = updater;
		_update  = update;
	}

	public async ValueTask Get(ExternalProcess parameter)
	{
		parameter.Updates.Add(_update.Get());
		await _updater.Await(parameter);
	}
}
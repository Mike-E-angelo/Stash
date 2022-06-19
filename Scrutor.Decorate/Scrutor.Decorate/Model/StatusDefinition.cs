using DragonSpark.Compose;
using DragonSpark.Model.Operations;
using DragonSpark.Model.Selection;

namespace Scrutor.Decorate.Model;

public class StatusDefinition : ISelect<IOperation<ExternalProcess>, IStatus>
{
	readonly IUpdate _update;

	protected StatusDefinition(ProcessStatus status, string? message = null) : this(new Update(status, message)) {}

	protected StatusDefinition(IUpdate update) => _update = update;

	public IStatus Get(IOperation<ExternalProcess> parameter) => new Status(_update, parameter);

	public IStatus Get<T>(ISaveProcess<T> parameter) where T : ExternalProcess
		=> Get(Start.A.Selection<ExternalProcess>().By.Cast<T>().Select(parameter).Out());
}
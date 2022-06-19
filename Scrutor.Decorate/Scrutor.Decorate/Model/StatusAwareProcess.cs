namespace Scrutor.Decorate.Model;

public sealed class StatusAwareProcess : StatusAwareProcess<DepositOrder>, IProcess
{
	public StatusAwareProcess(IProcess previous, Log log) : base(previous, log) {}
}
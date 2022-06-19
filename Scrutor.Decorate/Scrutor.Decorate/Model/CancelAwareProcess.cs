namespace Scrutor.Decorate.Model;

sealed class CancelAwareProcess : CancelAwareProcess<DepositOrder>, IProcess
{
	public CancelAwareProcess(IProcess previous, UpdateProcessStatus status) : base(previous, status) {}
}
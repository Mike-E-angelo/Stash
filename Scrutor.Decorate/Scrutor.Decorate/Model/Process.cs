namespace Scrutor.Decorate.Model;

sealed class Process : Process<DepositOrder>, IProcess
{
	public Process(Saving repository, ProcessPlan plan) : base(repository, plan) {}
}
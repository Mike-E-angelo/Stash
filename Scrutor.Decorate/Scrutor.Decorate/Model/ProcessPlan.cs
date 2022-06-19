namespace Scrutor.Decorate.Model;

sealed class ProcessPlan : ProcessPlan<DepositOrder>
{
	public ProcessPlan(DepositStartedStep started, CompleteDepositStep complete) : base(started, complete) {}
}
namespace Scrutor.Decorate.Model;

sealed class UpdateProcessStatus : UpdateProcessStatus<DepositOrder>
{
	public UpdateProcessStatus(ISaveProcess edit) : base(edit) {}
}
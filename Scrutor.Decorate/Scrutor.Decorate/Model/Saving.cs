using DragonSpark.Application.Entities.Editing;

namespace Scrutor.Decorate.Model;

sealed class Saving : Saving<DepositOrder>
{
	public Saving(ISaveProcess process, Save<DepositOrder> save) : base(process, save) {}
}
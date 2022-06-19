using DragonSpark.Compose;
using DragonSpark.Model.Operations;
using System.Runtime.InteropServices;

namespace Scrutor.Decorate.Model;

[Guid("8944D272-3B49-46A7-805E-FFF61B196149")]
public sealed class CompleteDepositStep : Model.Compose.Step<DepositOrder>
{
	public CompleteDepositStep(ICompleteDeposit operation) : this(operation, A.Type<CompleteDepositStep>().GUID) {}

	public CompleteDepositStep(IOperation<DepositOrder> operation, Guid identifier)
		: base(operation.Identified(identifier), "Completing Deposit...") {}
}
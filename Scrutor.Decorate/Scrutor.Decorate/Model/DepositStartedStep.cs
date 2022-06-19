using DragonSpark.Compose;
using DragonSpark.Model.Operations;
using System.Runtime.InteropServices;

namespace Scrutor.Decorate.Model;

[Guid("312FB684-28CA-4150-940A-6DCDA5788A24")]
public sealed class DepositStartedStep : ConfiguredFirstStep<DepositOrder>
{
	public DepositStartedStep(IDepositStarted operation) : this(operation, A.Type<DepositStartedStep>().GUID) {}

	public DepositStartedStep(IOperation<DepositOrder> operation, Guid identifier)
		: base(operation.Identified(identifier), "Starting Deposit...") {}
}
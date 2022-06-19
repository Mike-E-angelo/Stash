using DragonSpark.Model.Operations;
using DragonSpark.Model.Selection;

namespace Scrutor.Decorate.Model;

public interface IComplete<T> : ISelect<StartSubsequentStep<T>, IOperation<T>> where T : ExternalProcess {}
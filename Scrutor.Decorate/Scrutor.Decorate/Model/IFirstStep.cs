using DragonSpark.Model.Selection;

namespace Scrutor.Decorate.Model;

public interface IFirstStep<T> : ISelect<Saving<T>, StartSubsequentStep<T>> where T : ExternalProcess {}
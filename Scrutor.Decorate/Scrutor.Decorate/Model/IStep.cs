using DragonSpark.Model.Selection.Alterations;

namespace Scrutor.Decorate.Model;

public interface IStep<T> : IAlteration<StartSubsequentStep<T>> where T : ExternalProcess {}
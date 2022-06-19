namespace Scrutor.Decorate.Model;

public class CompleteContext<T> : DragonSpark.Model.Results.Instance<Await<T>> where T : ExternalProcess
{
	public CompleteContext(Await<T> instance) : base(instance) {}
}
namespace EfCore.CompiledQuery.ArgumentException.Model
{
	sealed class SubjectsNotTwo : EvaluateToArray<Context, None, Subject>
	{
		public SubjectsNotTwo(IContexts<Context> contexts, IQuery<None, Subject> query) : base(contexts, query) {}
	}
}
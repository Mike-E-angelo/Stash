using EfCore.CompiledQueries.BasicExpression.Model;

namespace EfCore.CompiledQueries.BasicExpression
{
	sealed class SubjectsNotTwo : EvaluateToArray<Context, None, Subject>
	{
		public SubjectsNotTwo(IContexts<Context> contexts) : this(contexts, Query.Default) {}

		public SubjectsNotTwo(IContexts<Context> contexts, IQuery<None, Subject> query) : base(contexts, query) {}
	}
}
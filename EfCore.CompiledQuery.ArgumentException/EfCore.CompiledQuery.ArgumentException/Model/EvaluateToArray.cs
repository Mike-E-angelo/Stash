using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public class EvaluateToArray<TContext, TIn, T> : EvaluateToArray<TIn, T> where TContext : DbContext where T : class
	{
		protected EvaluateToArray(IContexts<TContext> contexts, IQuery<TIn, T> query)
			: this(new Invoke<TContext, TIn, T>(contexts, query)) {}

		public EvaluateToArray(IInvoke<TIn, T> invoke) : base(invoke) {}
	}
}
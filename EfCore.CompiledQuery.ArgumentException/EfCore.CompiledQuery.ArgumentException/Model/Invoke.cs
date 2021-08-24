using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public class Invoke<TContext, T> : Invoke<TContext, None, T> where TContext : DbContext where T : class
	{
		public Invoke(IContexts<TContext> contexts, IQuery<None, T> query) : base(contexts, query) {}

		public Invoke(IContexts<TContext> contexts, IForm<None, T> form) : base(contexts, form) {}
	}
}
using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public class Invoke<TContext, T> : Invoke<TContext, None, T> where TContext : DbContext where T : class
	{
		public Invoke(IContexts<TContext> contexts, IQuery<None, T> query) : base(contexts, query) {}

		public Invoke(IContexts<TContext> contexts, IForm<None, T> form) : base(contexts, form) {}
	}

	public class Invoke<TContext, TIn, T> : IInvoke<TIn, T> where TContext : DbContext where T : class
	{
		readonly IContexts<TContext> _contexts;
		readonly IForm<TIn, T>       _form;

		public Invoke(IContexts<TContext> contexts, IQuery<TIn, T> query) : this(contexts, new Form<TIn, T>(query)) {}

		public Invoke(IContexts<TContext> contexts, IForm<TIn, T> form)
		{
			_contexts = contexts;
			_form     = form;
		}

		public Invocation<T> Get(TIn parameter)
		{
			var context = _contexts.Get();
			var compile = _form.Get(new In<TIn>(context, parameter));
			return new(context, compile);
		}
	}

}
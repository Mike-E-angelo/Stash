using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public class Query<TIn, T> : Instance<Expression<Func<DbContext, TIn, IQueryable<T>>>>, IQuery<TIn, T>
		where T : class
	{
		protected Query(Expression<Func<DbContext, TIn, IQueryable<T>>> instance) : base(instance) {}
	}

	public class Append<TIn, T> : Query<TIn, T> where T : class
	{
		public Append(Func<IQueryable<T>, IQueryable<T>> select) : base((context, @in) => select(context.Set<T>())) {}

		public Append(Func<DbContext, IQueryable<T>, IQueryable<T>> select)
			: this((context, _, queryable) => select(context, queryable)) {}

		public Append(Func<DbContext, TIn, IQueryable<T>, IQueryable<T>> select)
			: base((context, @in) => select(context, @in, context.Set<T>())) {}
	}

	public class EvaluateToArray<TIn, T> : Evaluate<TIn, T, T[]>
	{
		protected EvaluateToArray(IInvoke<TIn, T> invoke) : base(invoke, ToArray<T>.Default) {}
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
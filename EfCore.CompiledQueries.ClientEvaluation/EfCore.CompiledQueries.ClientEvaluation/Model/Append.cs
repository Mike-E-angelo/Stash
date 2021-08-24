using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public class Append<TIn, T> : Query<TIn, T> where T : class
	{
		public Append(Func<IQueryable<T>, IQueryable<T>> select) : base((context, @in) => select(context.Set<T>())) {}

		public Append(Func<DbContext, IQueryable<T>, IQueryable<T>> select)
			: this((context, _, queryable) => select(context, queryable)) {}

		public Append(Func<DbContext, TIn, IQueryable<T>, IQueryable<T>> select)
			: base((context, @in) => select(context, @in, context.Set<T>())) {}
	}
}
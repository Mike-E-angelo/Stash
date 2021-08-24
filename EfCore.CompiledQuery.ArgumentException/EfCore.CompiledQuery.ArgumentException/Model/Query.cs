using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	sealed class Query : Append<Subject>
	{
		public static Query Default { get; } = new Query();

		Query() : base(q => q.Where(x => x.Name != "Two")) {}
	}

	public class Query<TIn, T> : Instance<Expression<Func<DbContext, TIn, IQueryable<T>>>>, IQuery<TIn, T>
		where T : class
	{
		protected Query(Expression<Func<DbContext, TIn, IQueryable<T>>> instance) : base(instance) {}
	}
}
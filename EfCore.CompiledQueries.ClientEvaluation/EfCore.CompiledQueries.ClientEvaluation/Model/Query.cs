using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public class Query<TIn, T> : Instance<Expression<Func<DbContext, TIn, IQueryable<T>>>>, IQuery<TIn, T>
		where T : class
	{
		protected Query(Expression<Func<DbContext, TIn, IQueryable<T>>> instance) : base(instance) {}
	}
}
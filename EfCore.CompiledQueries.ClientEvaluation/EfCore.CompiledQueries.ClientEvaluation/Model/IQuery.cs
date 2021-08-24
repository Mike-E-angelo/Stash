using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface IQuery<TIn, T> : IResult<Expression<Func<DbContext, TIn, IQueryable<T>>>> where T : class {}
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	sealed class Form<TIn, T> : IForm<TIn, T> where T : class
	{
		readonly Func<DbContext, TIn, IAsyncEnumerable<T>> _select;

		public Form(IQuery<TIn, T> query) : this(query.Get()) {}

		public Form(Expression<Func<DbContext, TIn, IQueryable<T>>> expression)
			: this(EF.CompileAsyncQuery(expression)) {}

		public Form(Func<DbContext, TIn, IAsyncEnumerable<T>> select) => _select = @select;

		public IAsyncEnumerable<T> Get(In<TIn> parameter) => _select(parameter.Context, parameter.Parameter);
	}
}
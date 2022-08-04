using EfCore.CompiledQueries.BasicExpression.Model;
using System;
using System.Linq;

namespace EfCore.CompiledQueries.BasicExpression
{
	sealed class Query : Query<None, Subject>
	{
		public static Query Default { get; } = new Query();

		Query() : this(Where.Default.Get) {}

		Query(Func<IQueryable<Subject>, IQueryable<Subject>> select) : base((q, _) => select(q.Set<Subject>())) {}
	}
}
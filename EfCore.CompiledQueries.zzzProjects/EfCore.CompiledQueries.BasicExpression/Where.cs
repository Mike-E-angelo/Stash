using EfCore.CompiledQueries.BasicExpression.Model;
using System.Linq;

namespace EfCore.CompiledQueries.BasicExpression
{
	sealed class Where : ISelect<IQueryable<Subject>, IQueryable<Subject>>
	{
		public static Where Default { get; } = new Where();

		Where() {}

		public IQueryable<Subject> Get(IQueryable<Subject> parameter) => parameter.Where(x => x.Name != "Two");
	}
}
using EfCore.CompiledQueries.ClientEvaluation.Model;
using System.Linq;

namespace EfCore.CompiledQueries.ClientEvaluation
{
	sealed class Where : ISelect<IQueryable<Subject>, IQueryable<Subject>>
	{
		public static Where Default { get; } = new Where();

		Where() {}

		public IQueryable<Subject> Get(IQueryable<Subject> parameter) => parameter.Where(x => x.Name != "Two");
	}
}
using System.Linq;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface ISelector<TIn, out T> : ISelect<In<TIn>, IQueryable<T>> {}
}
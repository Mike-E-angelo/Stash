using System.Linq;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface ISelector<TIn, out T> : ISelect<In<TIn>, IQueryable<T>> {}
}
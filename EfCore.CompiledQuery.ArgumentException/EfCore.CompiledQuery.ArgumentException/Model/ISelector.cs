using System.Linq;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface ISelector<TIn, out T> : ISelect<In<TIn>, IQueryable<T>> {}
}
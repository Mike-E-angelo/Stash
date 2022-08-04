using System.Threading.Tasks;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface ISelecting<in TIn, TOut> : ISelect<TIn, ValueTask<TOut>> {}
}
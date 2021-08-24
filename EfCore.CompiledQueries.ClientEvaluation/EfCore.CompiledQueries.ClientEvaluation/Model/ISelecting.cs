using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface ISelecting<in TIn, TOut> : ISelect<TIn, ValueTask<TOut>> {}
}
using System.Threading.Tasks;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface ISelecting<in TIn, TOut> : ISelect<TIn, ValueTask<TOut>> {}
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	sealed class ToArray<T> : IEvaluate<T, T[]>
	{
		public static ToArray<T> Default { get; } = new ToArray<T>();

		ToArray() {}

		public ValueTask<T[]> Get(IAsyncEnumerable<T> parameter) => parameter.ToArrayAsync();
	}
}
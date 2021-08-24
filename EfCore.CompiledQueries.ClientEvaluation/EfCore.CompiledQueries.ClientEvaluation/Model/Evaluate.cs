using System.Threading.Tasks;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public class Evaluate<TIn, T, TResult> : ISelecting<TIn, TResult>
	{
		readonly IInvoke<TIn, T>       _invoke;
		readonly IEvaluate<T, TResult> _evaluate;

		protected Evaluate(IInvoke<TIn, T> invoke, IEvaluate<T, TResult> evaluate)
		{
			_invoke   = invoke;
			_evaluate = evaluate;
		}

		public async ValueTask<TResult> Get(TIn parameter)
		{
			await using var invocation = _invoke.Get(parameter);
			var             result     = await _evaluate.Get(invocation.Elements);
			return result;
		}
	}
}
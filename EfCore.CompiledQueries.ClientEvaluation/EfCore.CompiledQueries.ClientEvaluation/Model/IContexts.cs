using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public interface IContexts<out T> : IResult<T> where T : DbContext {}
}
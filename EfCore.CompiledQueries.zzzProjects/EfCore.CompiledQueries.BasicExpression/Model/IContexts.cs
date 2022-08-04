using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public interface IContexts<out T> : IResult<T> where T : DbContext {}
}
using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public interface IContexts<out T> : IResult<T> where T : DbContext {}
}
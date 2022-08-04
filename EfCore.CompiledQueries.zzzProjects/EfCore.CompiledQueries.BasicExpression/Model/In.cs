using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.BasicExpression.Model
{
	public readonly record struct In<T>(DbContext Context, T Parameter);
}
using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	public readonly record struct In<T>(DbContext Context, T Parameter);
}
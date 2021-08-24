using Microsoft.EntityFrameworkCore;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public readonly record struct In<T>(DbContext Context, T Parameter);
}
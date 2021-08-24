using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public class Append<T> : Append<None, T> where T : class
	{
		public Append(Func<IQueryable<T>, IQueryable<T>> @select) : base(@select) {}

		public Append(Func<DbContext, IQueryable<T>, IQueryable<T>> @select) : base(@select) {}
	}
}
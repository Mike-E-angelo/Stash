using System;
using System.Linq;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	public class Selected<TIn, T> : Append<TIn, T> where T : class
	{
		public Selected(Func<In<TIn>, IQueryable<T>> select)
			: base((context, @in, _) => select(new In<TIn>(context, @in))) {}
	}
}
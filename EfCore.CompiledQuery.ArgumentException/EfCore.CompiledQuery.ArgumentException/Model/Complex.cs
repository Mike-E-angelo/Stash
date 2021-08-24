using System;
using System.Linq;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	sealed class Complex : Append<Subject>
	{
		public static Complex Default { get; } = new Complex();

		Complex() : this(Selection.Default.Get) {}

		public Complex(Func<IQueryable<Subject>, IQueryable<Subject>> @select)
			: base((_, subjects) => select(subjects)) {}
	}
}
using System;

namespace EfCore.ScopedVsTransaction
{
	public sealed class Subject
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = default!;
	}
}
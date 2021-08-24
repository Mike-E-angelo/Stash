using System;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	sealed class Subject
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = default!;
	}
}
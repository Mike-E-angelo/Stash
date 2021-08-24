using System;

namespace EfCore.CompiledQueries.ClientEvaluation.Model
{
	sealed class Subject
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = default!;
	}
}
using System.Linq;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	sealed class Query : Append<Subject>
	{
		public static Query Default { get; } = new Query();

		Query() : base(q => q.Where(x => x.Name != "Two")) {}
	}
}
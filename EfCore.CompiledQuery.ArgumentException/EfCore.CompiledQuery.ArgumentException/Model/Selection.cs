using System.Linq;

namespace EfCore.CompiledQuery.ArgumentException.Model
{
	sealed class Selection : Select<IQueryable<Subject>, IQueryable<Subject>>
	{
		public static Selection Default { get; } = new Selection();

		Selection() : base(q => q.Where(x => x.Name != "Two")) {}
	}
}
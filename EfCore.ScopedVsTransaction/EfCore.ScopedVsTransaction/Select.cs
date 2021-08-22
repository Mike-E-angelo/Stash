using System.Linq;

namespace EfCore.ScopedVsTransaction
{
	sealed class Select
	{
		public static Select Default { get; } = new Select();

		Select() {}

		public IQueryable<Subject> Get(IQueryable<Subject> parameter) => parameter.Where(x => x.Name != "Two")
		                                                                          .Where(x => x.Name != "Two")
		                                                                          .Where(x => x.Name != "Two");
	}
}
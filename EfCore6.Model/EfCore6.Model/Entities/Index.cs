using Microsoft.EntityFrameworkCore;

namespace EfCore6.Model.Entities
{
	[Owned]
	public class Index
	{
		public virtual uint Number { get; set; }
	}
}
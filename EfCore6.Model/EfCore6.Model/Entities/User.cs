namespace EfCore6.Model.Entities
{
	public class User : DragonSpark.Application.Security.Identity.IdentityUser
	{
		public virtual bool Enabled { get; set; } = true;

		public Definition.Definition Definition { get; set; } = default!;

		public Index Index { get; set; } = default!;
	}
}
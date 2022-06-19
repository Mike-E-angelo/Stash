using Microsoft.EntityFrameworkCore;

namespace Scrutor.Decorate.Model;

[Owned]
public class CompletedStep
{
	public Guid Identifier { get; set; }
}
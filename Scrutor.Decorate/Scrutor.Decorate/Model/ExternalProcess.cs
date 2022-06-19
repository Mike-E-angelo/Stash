namespace Scrutor.Decorate.Model;

public abstract class ExternalProcess
{
	public Guid Id { get; set; }

	public DateTimeOffset Created { get; set; }

	public ICollection<ProcessUpdate> Updates { get; set; } = default!;

	public ICollection<CompletedStep> CompletedSteps { get; set; } = default!;
}
namespace Scrutor.Decorate.Model;

public class ProcessUpdate
{
	public long Id { get; set; }

	public DateTimeOffset Created { get; set; }

	public ProcessStatus Status { get; set; }

	public string? Message { get; set; }
}
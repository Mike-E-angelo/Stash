namespace Scrutor.Decorate.Model;

sealed class ProcessingDefinition : StatusDefinition
{
	public static ProcessingDefinition Default { get; } = new();

	ProcessingDefinition() : base(ProcessStatus.Processing) {}

	public ProcessingDefinition(string message) : base(ProcessStatus.Processing, message) {}
}
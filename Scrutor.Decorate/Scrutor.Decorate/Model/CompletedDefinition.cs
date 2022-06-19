namespace Scrutor.Decorate.Model;

sealed class CompletedDefinition : StatusDefinition
{
	public static CompletedDefinition Default { get; } = new();

	CompletedDefinition() : base(ProcessStatus.Completed) {}

	public CompletedDefinition(string message) : base(ProcessStatus.Completed, message) {}
}
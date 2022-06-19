namespace Scrutor.Decorate.Model;

sealed class ErrorDefinition : StatusDefinition
{
	public static ErrorDefinition Default { get; } = new();

	ErrorDefinition() : base(ProcessStatus.Error, "A problem occurred while processing.") {}

	public ErrorDefinition(string message) : base(ProcessStatus.Error, message) {}
}
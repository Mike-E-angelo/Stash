namespace Scrutor.Decorate.Model;

public readonly record struct UpdateProcessStatusInput<T>(T Process, ProcessStatus Status, string Message)
	where T : ExternalProcess;
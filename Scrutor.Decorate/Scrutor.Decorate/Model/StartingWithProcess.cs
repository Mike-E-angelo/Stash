namespace Scrutor.Decorate.Model;

public sealed class StartingWithProcess<T> where T : ExternalProcess
{
	readonly Saving<T> _saving;

	public StartingWithProcess(Saving<T> saving) => _saving = saving;

	public StartingMessage<T> With => new(_saving);
}
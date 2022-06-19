namespace Scrutor.Decorate.Model;

public sealed class StartingProcess<T> where T : ExternalProcess
{
	readonly Saving<T> _saving;

	public StartingProcess(Saving<T> saving) => _saving = saving;

	public StartingWithProcess<T> Starting => new(_saving);
}
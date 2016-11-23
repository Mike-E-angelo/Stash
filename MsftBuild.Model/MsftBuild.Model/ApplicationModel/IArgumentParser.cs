namespace MsftBuild.Model.ApplicationModel
{
	public interface IArgumentParser
	{
		ApplicationArguments Parse( string[] arguments );
	}
}
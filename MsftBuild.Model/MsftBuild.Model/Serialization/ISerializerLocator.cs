namespace MsftBuild.Model.Serialization
{
	public interface ISerializerLocator
	{
		ISerializer Locate( string fileName );
	}
}
using System.IO;

namespace MsftBuild.Model.Serialization
{
	public interface ISerializer
	{
		T Load<T>( Stream data );

		string Save<T>( T item );
	}
}
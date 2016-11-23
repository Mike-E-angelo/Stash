using System.IO;
using System.Runtime.Serialization;

namespace MsftBuild.Model.Serialization
{
	public sealed class XmlSerializer : ISerializer
	{
		public static ISerializer Default { get; } = new XmlSerializer();
		XmlSerializer() {}

		public T Load<T>( Stream data ) => (T)new DataContractSerializer( typeof(T) ).ReadObject( data );

		public string Save<T>( T item )
		{
			var stream = new MemoryStream();
			var type = typeof(T) == typeof(object) ? item.GetType() : typeof(T);
			new DataContractSerializer( type ).WriteObject( stream, item );
			stream.Seek( 0, SeekOrigin.Begin );
			var result = new StreamReader( stream ).ReadToEnd();
			return result;
		}
	}
}
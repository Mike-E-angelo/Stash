using Newtonsoft.Json;
using System.IO;

namespace MsftBuild.Model.Serialization
{
	public sealed class JsonSerializer : ISerializer
	{
		public static JsonSerializer Default { get; } = new JsonSerializer();
		JsonSerializer() {}

		public T Load<T>( Stream data ) => 
			JsonConvert.DeserializeObject<T>( new StreamReader( data ).ReadToEnd() );

		public string Save<T>( T item ) => 
			JsonConvert.SerializeObject( item );
	}
}
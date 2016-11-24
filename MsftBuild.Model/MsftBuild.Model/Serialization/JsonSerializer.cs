using Newtonsoft.Json;
using System.IO;

namespace MsftBuild.Model.Serialization
{
	public sealed class JsonSerializer : ISerializer
	{
		public static JsonSerializer Default { get; } = new JsonSerializer();
		JsonSerializer() : this( KnownJsonConverters.Default ) {}

		readonly JsonSerializerSettings settings;

		public JsonSerializer( System.Collections.Generic.IList<JsonConverter> converters ) : this( new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Converters = converters, ContractResolver = ContractResolver.Default } ) {}

		public JsonSerializer( JsonSerializerSettings settings )
		{
			this.settings = settings;
		}

		public T Load<T>( Stream data ) => JsonConvert.DeserializeObject<T>( new StreamReader( data ).ReadToEnd(), settings );

		public string Save<T>( T item ) => JsonConvert.SerializeObject( item, typeof(T), settings );
	}
}
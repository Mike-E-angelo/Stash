using Newtonsoft.Json;
using System;

namespace MsftBuild.Model.Serialization
{
	public class TypeNameConverter : JsonConverter
	{
		public override bool CanConvert( Type objectType ) => objectType == typeof(Type);

		public override void WriteJson( JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer ) => serializer.Serialize( writer, serializer.GetTypeName( (Type)value ) );
		
		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer ) => Type.GetType( (string)reader.Value );
	}
}
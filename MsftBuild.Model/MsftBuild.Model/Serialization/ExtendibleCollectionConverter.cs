using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MsftBuild.Model.Serialization
{
	public class ExtendibleCollectionConverter : JsonConverter
	{
		public static ExtendibleCollectionConverter Default { get; } = new ExtendibleCollectionConverter();
		ExtendibleCollectionConverter() : this( typeof(IList<IFile>) ) {}

		readonly Type collectionType;

		public ExtendibleCollectionConverter( Type collectionType )
		{
			this.collectionType = collectionType;
		}

		public override bool CanConvert( Type objectType ) => collectionType.IsAssignableFrom( objectType );

		public override object ReadJson(
			JsonReader reader, Type objectType,
			object existingValue, Newtonsoft.Json.JsonSerializer serializer )
		{
			var restore = new { serializer.Binder, serializer.MetadataPropertyHandling };
			serializer.Binder = new Binder( restore.Binder, collectionType, typeof(ExtendedEnumerableSurrogate) );
			serializer.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
			var surrogate = serializer.Deserialize<ExtendedEnumerableSurrogate>( reader );
			serializer.Binder = restore.Binder;
			serializer.MetadataPropertyHandling = restore.MetadataPropertyHandling;

			var result = new CollectionFactory( serializer ).Create( surrogate );
			return result;
		}

		sealed class Binder : SerializationBinder
		{
			readonly SerializationBinder inner;
			readonly Type monitoredType;
			readonly Type replaceType;

			public Binder( SerializationBinder inner, Type monitoredType, Type replaceType )
			{
				this.inner = inner;
				this.monitoredType = monitoredType;
				this.replaceType = replaceType;
			}

			public override void BindToName( Type serializedType, out string assemblyName, out string typeName ) => inner.BindToName( serializedType, out assemblyName, out typeName );

			public override Type BindToType( string assemblyName, string typeName )
			{
				var bindToType = inner.BindToType( assemblyName, typeName );
				var result = monitoredType.IsAssignableFrom( bindToType ) ? replaceType : bindToType;
				return result;
			}
		}

		public override void WriteJson( JsonWriter writer, object value,
										Newtonsoft.Json.JsonSerializer serializer )
		{
			var type = value.GetType();
			var provider = serializer.ContractResolver as IPropertyProvider;
			var source = provider != null ? CreateSurrogate( provider.GetProperties( type ), type, (IEnumerable)value ) : value;
			serializer.Serialize( writer, source );
		}


		static object CreateSurrogate( IEnumerable<JsonProperty> properties, Type type, IEnumerable value )
		{
			var data = properties.ToDictionary( property => property.PropertyName, property =>
																				   {
																					   var item = property.ValueProvider.GetValue( value );
																					   return (object)(JToken)( item as string ) ?? new JObject( item );
																				   } );
			var result = new ExtendedEnumerableSurrogate( value, data, type );
			return result;
		}
	}
}
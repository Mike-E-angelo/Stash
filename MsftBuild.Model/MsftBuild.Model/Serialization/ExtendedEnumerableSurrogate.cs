using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MsftBuild.Model.Serialization
{
	[JsonObject( MemberSerialization.OptIn, ItemTypeNameHandling = TypeNameHandling.None )]
	public sealed class ExtendedEnumerableSurrogate
	{
		[JsonConstructor]
		public ExtendedEnumerableSurrogate() {}

		public ExtendedEnumerableSurrogate( IEnumerable items, IDictionary<string, object> properties, Type referencedType )
		{
			Properties = properties;
			Items = items.Cast<object>().ToArray();
			ReferencedType = referencedType;
		}

		[JsonProperty( "$type" ), JsonConverter( typeof(TypeNameConverter) )]
		public Type ReferencedType { get; set; }

		[JsonProperty( "$values" )]
		public object[] Items { get; set; } = new object[0];
		public bool ShouldSerializeItems() => Items.Any();

		[JsonExtensionData]
		public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
		public bool ShouldSerializeProperties() => Properties.Any();
	}
}
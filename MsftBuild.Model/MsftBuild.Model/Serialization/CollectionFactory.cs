using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace MsftBuild.Model.Serialization
{
	public sealed class CollectionFactory
	{
		readonly IContractResolver resolver;
		readonly Newtonsoft.Json.JsonSerializer serializer;

		public CollectionFactory( Newtonsoft.Json.JsonSerializer serializer ) : this( serializer, serializer.ContractResolver ) {}

		public CollectionFactory( Newtonsoft.Json.JsonSerializer serializer, IContractResolver resolver )
		{
			this.resolver = resolver;
			this.serializer = serializer;
		}

		public object Create( ExtendedEnumerableSurrogate parameter )
		{
			var instance = (IList)Activator.CreateInstance( parameter.ReferencedType );

			var arrayType = resolver.ResolveContract( parameter.ReferencedType ) as JsonArrayContract;
			if ( arrayType != null )
			{
				foreach ( var item in parameter.Items.Cast<JObject>() )
				{
					instance.Add( item.ToObject( arrayType.CollectionItemType, serializer ) );
				}
				var result = parameter.Properties.Into( instance );
				return result;
			}
			return null;
		}
	}
}
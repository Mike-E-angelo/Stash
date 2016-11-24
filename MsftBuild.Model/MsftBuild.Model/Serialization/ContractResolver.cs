using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MsftBuild.Model.Serialization
{
	sealed class ContractResolver : DefaultContractResolver, IPropertyProvider
	{
		public static ContractResolver Default { get; } = new ContractResolver();
		ContractResolver() {}

		public IEnumerable<JsonProperty> GetProperties( Type type ) => CreateProperties( type, MemberSerialization.OptOut ).Where( property => property.DeclaringType == type );
	}
}
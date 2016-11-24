using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace MsftBuild.Model.Serialization
{
	public interface IPropertyProvider
	{
		IEnumerable<JsonProperty> GetProperties( Type type );
	}
}
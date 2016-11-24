using Newtonsoft.Json;

namespace MsftBuild.Model.Serialization
{
	public sealed class KnownJsonConverters : System.Collections.Generic.List<JsonConverter>
	{
		public static KnownJsonConverters Default { get; } = new KnownJsonConverters();
		KnownJsonConverters() : base( new JsonConverter[] { ExtendibleCollectionConverter.Default } ) {}
	}
}
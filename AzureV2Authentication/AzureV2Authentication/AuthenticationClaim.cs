using Newtonsoft.Json;

namespace AzureFunctionsV2Authentication
{
	public class AuthenticationClaim
	{
		[JsonProperty("typ")]
		public string Type { get; set; }
		[JsonProperty("val")]
		public string Value { get; set; }
	}
}
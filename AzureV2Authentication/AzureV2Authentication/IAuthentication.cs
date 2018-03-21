using Refit;
using System.Threading.Tasks;

namespace AzureFunctionsV2Authentication
{
	interface IAuthentication
	{
		[Get("/.auth/me")]
		Task<Authentication[]> GetCurrentAuthentication();
	}
}
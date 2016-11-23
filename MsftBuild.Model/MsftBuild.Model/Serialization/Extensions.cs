using System.IO;
using System.Text;

namespace MsftBuild.Model.Serialization
{
	public static class Extensions
	{
		public static T Load<T>( this ISerializer @this, string data ) => @this.Load<T>( new MemoryStream( Encoding.Default.GetBytes( data ) ) );
	}
}
using System.IO;
using System.Xaml;

namespace MsftBuild.Model.Serialization
{
	public sealed class XamlSerializer : ISerializer
	{
		public static ISerializer Default { get; } = new XamlSerializer();
		XamlSerializer() {}

		public T Load<T>( Stream data ) => (T)XamlServices.Load( data );

		public string Save<T>( T item ) => XamlServices.Save( item );
	}
}
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	internal class XamlRootObjectProvider : IRootObjectProvider
	{
		public object RootObject
		{
			get;
			private set;
		}
		public XamlRootObjectProvider(object rootObject)
		{
			this.RootObject = rootObject;
		}
	}
}

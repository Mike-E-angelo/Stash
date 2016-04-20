using System;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	public interface IContentPropertyService
	{
		string GetContentProperty( Type target, IServiceProvider serviceProvider );
	}

	public class ContentPropertyService : IContentPropertyService
	{
		public static ContentPropertyService Instance
		{
			get { return InstanceField; }
		}	static readonly ContentPropertyService InstanceField = new ContentPropertyService();

		public virtual string GetContentProperty( Type target, IServiceProvider serviceProvider )
		{
			var attribute = target.GetTypeInfo().GetCustomAttribute<ContentPropertyAttribute>();
			var result = attribute != null ? attribute.Name : null;
			return result;
		}
	}
}
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("Name")]
	public class ReferenceExtension : IMarkupExtension
	{
		public string Name
		{
			get;
			set;
		}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
			{
				throw new ArgumentException("serviceProvider does not provide an IProvideValueTarget");
			}
			INameScope nameScope = null;
			INameScopeProvider nameScopeProvider = serviceProvider.GetService(typeof(INameScopeProvider)) as INameScopeProvider;
			if (nameScopeProvider != null)
			{
				nameScope = nameScopeProvider.NameScope;
			}
			INameScope nameScope2 = provideValueTarget.TargetObject as INameScope;
			nameScope = (nameScope ?? nameScope2);
			if (nameScope == null)
			{
				throw new Exception("Can't resolve name on Element");
			}
			return nameScope.FindByName(this.Name);
		}
	}
}

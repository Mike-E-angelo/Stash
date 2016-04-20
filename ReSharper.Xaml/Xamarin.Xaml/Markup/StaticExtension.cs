using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	[ContentProperty("Member")]
	public class StaticExtension : IMarkupExtension
	{
		public string Member
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
			IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
			if (xamlTypeResolver == null)
			{
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			}
			if (string.IsNullOrEmpty(this.Member) || !this.Member.Contains("."))
			{
				throw new XamlParseException("Syntax for x:Static is [Member=][prefix:]typeName.staticMemberName");
			}
			int num = this.Member.LastIndexOf('.');
			string qualifiedTypeName = this.Member.Substring(0, num);
			string membername = this.Member.Substring(num + 1);
			Type type = xamlTypeResolver.Resolve(qualifiedTypeName, serviceProvider);
			PropertyInfo propertyInfo = type.GetRuntimeProperties().FirstOrDefault((PropertyInfo pi) => pi.Name == membername && pi.GetMethod.IsStatic);
			if (propertyInfo != null)
			{
				return propertyInfo.GetMethod.Invoke(null, new object[0]);
			}
			FieldInfo fieldInfo = type.GetRuntimeFields().FirstOrDefault((FieldInfo fi) => fi.Name == membername && fi.IsStatic);
			if (fieldInfo != null)
			{
				return fieldInfo.GetValue(null);
			}
			throw new XamlParseException(string.Format("No static member found for {0}", new object[]
			{
				this.Member
			}));
		}
	}
}

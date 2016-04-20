using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	internal sealed class MarkupExtensionParser : MarkupExpressionParser, IExpressionParser
	{
		private IMarkupExtension markupExtension;
		public object Parse(string match, ref string remaining, IServiceProvider serviceProvider)
		{
			IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
			if (match == "Binding")
			{
				this.markupExtension = new BindingExtension();
			}
			else if (match == "StaticResource")
			{
				this.markupExtension = new StaticResourceExtension();
			}
			else
			{
				if (xamlTypeResolver == null)
				{
					return null;
				}
				Type type;
				if (!xamlTypeResolver.TryResolve(match + "Extension", out type) && !xamlTypeResolver.TryResolve(match, out type))
				{
					throw new XamlParseException(string.Format("MarkupExtension not found for {0}", new object[]
					{
						match
					}));
				}
				this.markupExtension = (Activator.CreateInstance(type) as IMarkupExtension);
			}
			if (this.markupExtension == null)
			{
				throw new XamlParseException(string.Format("Missing public default constructor for MarkupExtension {0}", new object[]
				{
					match
				}));
			}
			if (remaining == "}")
			{
				return this.markupExtension.ProvideValue(serviceProvider);
			}
			char c;
			string nextPiece;
			while ((nextPiece = base.GetNextPiece(ref remaining, out c)) != null)
			{
				base.HandleProperty(nextPiece, serviceProvider, ref remaining, c != '=');
			}
			return this.markupExtension.ProvideValue(serviceProvider);
		}
		protected override void SetPropertyValue(string prop, string strValue, object value, IServiceProvider serviceProvider)
		{
			MethodInfo setMethod;
			if (prop == null)
			{
				Type type = this.markupExtension.GetType();
				prop = serviceProvider.GetService<IContentPropertyService>().GetContentProperty( type, serviceProvider );
				if ( prop == null )
				{
					return;
				}
				setMethod = type.GetRuntimeProperty(prop).SetMethod;
			}
			else
			{
				setMethod = this.markupExtension.GetType().GetRuntimeProperty(prop).SetMethod;
			}
			if (value == null && strValue != null)
			{
				value = strValue.ConvertTo(this.markupExtension.GetType().GetRuntimeProperty(prop).PropertyType, (Func<TypeConverter>)null, serviceProvider);
			}
			setMethod.Invoke(this.markupExtension, new object[]
			{
				value
			});
		}
	}
}

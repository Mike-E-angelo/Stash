using System;
using System.Reflection;
using System.Xml;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class XamlTypeResolver : IXamlTypeResolver
	{
		private readonly IXmlNamespaceResolver namespaceResolver;
		private readonly Assembly currentAssembly;
		public XamlTypeResolver(IXmlNamespaceResolver namespaceResolver, Assembly currentAssembly)
		{
			this.currentAssembly = currentAssembly;
			if (namespaceResolver == null)
			{
				throw new ArgumentNullException();
			}
			this.namespaceResolver = namespaceResolver;
		}
		public Type Resolve(string qualifiedTypeName, IServiceProvider serviceProvider = null)
		{
			string[] array = qualifiedTypeName.Split( ':' );
			if (array.Length > 2)
			{
				return null;
			}
			string text;
			string name;
			if (array.Length == 2)
			{
				text = array[0];
				name = array[1];
			}
			else
			{
				text = "";
				name = array[0];
			}
			IXmlLineInfo xmlInfo = null;
			if (serviceProvider != null)
			{
				IXmlLineInfoProvider xmlLineInfoProvider = serviceProvider.GetService<IXmlLineInfoProvider>();
				if (xmlLineInfoProvider != null)
				{
					xmlInfo = xmlLineInfoProvider.XmlLineInfo;
				}
			}
			string text2 = string.IsNullOrEmpty(text) ? string.Empty : this.namespaceResolver.LookupNamespace(text);
			if (text2 == null)
			{
				throw new XamlParseException(string.Format("No xmlns declaration for prefix \"{0}\"", text ), xmlInfo);
			}
			var provider = serviceProvider ?? Services.Default;
			var result = provider.GetService<ITypeResolutionService>().GetElementType( new XmlType( new XmlName(text2, name) ), xmlInfo, this.currentAssembly );
			return result;
		}
		public bool TryResolve(string qualifiedTypeName, out Type type)
		{
			bool result;
			try
			{
				type = Resolve(qualifiedTypeName);
				result = true;
			}
			catch
			{
				type = null;
				result = false;
			}
			return result;
		}
	}
}

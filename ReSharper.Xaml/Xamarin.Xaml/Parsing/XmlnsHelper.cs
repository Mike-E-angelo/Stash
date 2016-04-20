using System;

namespace Xamarin.Xaml
{
	public static class XmlnsHelper
	{
		public static bool IsCustom(string ns)
		{
			return ns == null || (ns != string.Empty && ns != KnownSchemas.Forms2014);
		}
		public static string ParseNamespaceFromXmlns(string xmlns)
		{
			string text = null;
			string result = null;
			string text2 = null;
			XmlnsHelper.ParseXmlns(xmlns, out text, out result, out text2);
			return result;
		}
		public static void ParseXmlns(string xmlns, out string typeName, out string ns, out string asm)
		{
			string text;
			asm = (text = null);
			string text2;
			ns = (text2 = text);
			typeName = text2;
			string[] array = xmlns.Split(new char[]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text3 = array[i];
				if (text3.StartsWith("clr-namespace:", StringComparison.Ordinal))
				{
					ns = text3.Substring(14, text3.Length - 14);
				}
				else if (text3.StartsWith("assembly=", StringComparison.Ordinal))
				{
					asm = text3.Substring(9, text3.Length - 9);
				}
				else
				{
					int num = text3.LastIndexOf(".", StringComparison.Ordinal);
					if (num > 0)
					{
						ns = text3.Substring(0, num);
						typeName = text3.Substring(num + 1, text3.Length - num - 1);
					}
					else
					{
						typeName = text3;
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Xamarin.Xaml
{
	public interface IAttributeParser
	{
		KeyValuePair<XmlName, INode>[] Parse( XmlReader reader );
	}

	public class AttributeParser : IAttributeParser
	{
		public static AttributeParser Instance
		{
			get { return InstanceField; }
		}	static readonly AttributeParser InstanceField = new AttributeParser();

		public KeyValuePair<XmlName, INode>[] Parse( XmlReader reader )
		{
			var list = new List<KeyValuePair<XmlName, INode>>();
			for ( var i = 0; i < reader.AttributeCount; i++ )
			{
				reader.MoveToAttribute( i );
				if ( reader.NamespaceURI != KnownSchemas.Xmlns )
				{
					var xKey = new XmlName( reader.NamespaceURI, reader.LocalName );
					object obj2 = reader.Value;
					switch ( reader.NamespaceURI )
					{
						case KnownSchemas.Xaml2006:
							var str = reader.Name;
							if ( str != null )
							{
								if ( str == "x:Key" )
								{
									xKey = XmlName.xKey;
									break;
								}
								if ( str == "x:Name" )
								{
									goto Label_009D;
								}
								if ( str == "x:Class" )
								{} // ?
							}
							continue;
					}
					goto Xaml2009;
					Label_009D:
					xKey = XmlName.xName;
					Xaml2009:
					if ( reader.NamespaceURI == KnownSchemas.Xaml2009 )
					{
						var name = reader.Name;
						if ( name == null )
						{
							continue;
						}
						if ( name != "x:Key" )
						{
							if ( name == "x:Name" )
							{
								goto Label_0116;
							}
							if ( name == "x:TypeArguments" )
							{
								goto Label_011E;
							}
							if ( ( name != "x:Class" ) && ( name == "x:FactoryMethod" ) )
							{
								goto Label_013E;
							}
							continue;
						}
						xKey = XmlName.xKey;
					}
					goto Label_0144;
					Label_0116:
					xKey = XmlName.xName;
					goto Label_0144;
					Label_011E:
					xKey = XmlName.xTypeArguments;
					obj2 = ConvertTypeArguments( (string)obj2, (IXmlNamespaceResolver)reader );
					goto Label_0144;
					Label_013E:
					xKey = XmlName.xFactoryMethod;
					Label_0144:
					var node = GetValueNode( obj2, reader );
					list.Add( new KeyValuePair<XmlName, INode>( xKey, node ) );
				}
			}
			reader.MoveToElement();
			return list.ToArray();
		}
		
		private static XmlType[] ConvertTypeArguments(string typeArguments, IXmlNamespaceResolver resolver)
		{
			var result = typeArguments.Split( ',' ).Select( s =>
			{
				var namespaceUri = s.Contains( ":" ) ? resolver.LookupNamespace( s.Split( ':' ).First() ) : string.Empty;
				return new XmlType( new XmlName( namespaceUri, s ) );
			} ).ToArray();
			return result;
		}

		private static IValueNode GetValueNode(object value, XmlReader reader)
		{
			string text = value as string;
			if (text != null && text.Trim().StartsWith("{}", StringComparison.Ordinal))
			{
				return new ValueNode(text.Substring(2), (IXmlNamespaceResolver)reader, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
			}
			if (text != null && text.Trim().StartsWith("{", StringComparison.Ordinal))
			{
				return new MarkupNode(text.Trim(), reader as IXmlNamespaceResolver, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
			}
			return new ValueNode(value, (IXmlNamespaceResolver)reader, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
		}
		
	}
}
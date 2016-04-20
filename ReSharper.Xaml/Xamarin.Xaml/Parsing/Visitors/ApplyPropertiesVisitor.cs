using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class ApplyPropertiesVisitor : XamlNodeVisitorBase
	{
		public static readonly IList<XmlName> Skips = new List<XmlName>
		{
			XmlName.xKey,
			XmlName.xTypeArguments,
			XmlName.xArguments,
			XmlName.xFactoryMethod,
			XmlName.xName
		};

		public ApplyPropertiesVisitor( HydratationContext context ) : base( true )
		{
			Context = context;
		}

		IDictionary<INode, object> Values
		{
			get { return Context.Values; }
		}

		HydratationContext Context { get; set; }

		public static bool TryGetPropertyName( INode node, INode parentNode, out XmlName name )
		{
			name = default(XmlName);
			var elementNode = parentNode as IElementNode;
			if ( elementNode == null )
			{
				return false;
			}
			foreach ( var current in elementNode.Properties.Where( current => current.Value == node ) )
			{
				name = current.Key;
				return true;
			}
			return false;
		}

		static bool IsCollectionItem( INode node, INode parentNode )
		{
			var listNode = parentNode as IListNode;
			return listNode != null && listNode.CollectionItems.Contains( node );
		}

		protected override void Visit( ValueNode node, INode parentNode )
		{
			var key = parentNode as IElementNode;
			var value = Values[node];
			var xamlelement = Values[parentNode];
			XmlName xmlName;
			if ( !TryGetPropertyName( node, parentNode, out xmlName ) )
			{
				var provider = Services.Create( Context, node );
				var name = provider.GetService<IContentPropertyService>().GetContentProperty( Context.Types[key], provider );

				if ( IsCollectionItem( node, parentNode ) && parentNode is IElementNode && name != null )
				{
					var xmlName2 = new XmlName( ( (ElementNode)parentNode ).XmlType.Name.NamespaceUri, name );
					if ( Skips.Contains( xmlName2 ) )
					{
						return;
					}
					SetPropertyValue( xamlelement, xmlName2, value, Context.RootElement, node, Context, node );
				}
				return;
			}
			if ( Skips.Contains( xmlName ) )
			{
				return;
			}
			SetPropertyValue( xamlelement, xmlName, value, Context.RootElement, node, Context, node );
		}

		protected override void Visit( ElementNode node, INode parentNode )
		{
			var key = parentNode as IElementNode;
			var provider = Services.Create( Context, node );
			var obj = provider.CheckValue( Values[node] );
			
			XmlName xmlName;
			if ( !TryGetPropertyName( node, parentNode, out xmlName ) )
			{
				if ( IsCollectionItem( node, parentNode ) && parentNode is IElementNode )
				{
					string contentName;
					if ( typeof(IEnumerable).GetTypeInfo().IsAssignableFrom( Context.Types[key].GetTypeInfo() ) )
					{
						var obj2 = Values[parentNode];
						if ( Context.Types[key] != typeof(ResourceDictionary) )
						{
							var methodInfo = Context.Types[key].GetRuntimeMethods().First( mi => mi.Name == "Add" && mi.GetParameters().Length == 1 );
							methodInfo.Invoke( obj2, new[]
							{
								obj
							} );
							return;
						}
					}
					else if ( ( contentName = provider.GetService<IContentPropertyService>().GetContentProperty( Context.Types[key], provider ) ) != null )
					{
						var xmlName2 = new XmlName( node.XmlType.Name.NamespaceUri, contentName );
						if ( Skips.Contains( xmlName2 ) )
						{
							return;
						}
						var xamlelement = Values[parentNode];
						SetPropertyValue( xamlelement, xmlName2, obj, Context.RootElement, node, Context, node );
						return;
					}
				}
				else if ( IsCollectionItem( node, parentNode ) && parentNode is ListNode )
				{
					var listNode = (ListNode)parentNode;
					var obj3 = Values[parentNode.Parent];
					if ( Skips.Contains( listNode.XmlName ) )
					{
						return;
					}
					var descriptor = DetermineMemberInformation( obj3.GetType(), listNode.XmlName.NamespaceUri, listNode.XmlName.LocalName, Context.RootElement.GetType().GetTypeInfo().Assembly, node );
					PropertyInfo propertyInfo = null;
					try
					{
						propertyInfo = descriptor.DeclaringType.GetRuntimeProperty( descriptor.MemberName );
					}
					catch ( AmbiguousMatchException )
					{
						foreach ( var current in 
							from prop in descriptor.DeclaringType.GetRuntimeProperties()
							where prop.Name == descriptor.MemberName
							select prop )
						{
							if ( propertyInfo == null || propertyInfo.DeclaringType.IsAssignableFrom( current.DeclaringType ) )
							{
								propertyInfo = current;
							}
						}
					}
					if ( propertyInfo == null )
					{
						throw new XamlParseException( string.Format( "Property {0} not found", descriptor.MemberName ), node );
					}
					MethodInfo getMethod;
					if ( !propertyInfo.CanRead || ( getMethod = propertyInfo.GetMethod ) == null )
					{
						throw new XamlParseException( string.Format( "Property {0} does not have an accessible getter", descriptor.MemberName ), node );
					}
					IEnumerable enumerable;
					if ( ( enumerable = ( getMethod.Invoke( obj3, new object[0] ) as IEnumerable ) ) == null )
					{
						throw new XamlParseException( string.Format( "Property {0} is null or is not IEnumerable", descriptor.MemberName ), node );
					}
					MethodInfo methodInfo2;
					if ( ( methodInfo2 = enumerable.GetType().GetRuntimeMethods().First( ( MethodInfo mi ) => mi.Name == "Add" && mi.GetParameters().Length == 1 ) ) == null )
					{
						throw new XamlParseException( string.Format( "Value of {0} does not have a Add() method", descriptor.MemberName ), node );
					}
					methodInfo2.Invoke( enumerable, new[]
					{
						Values[node]
					} );
				}
				return;
			}
			if ( Skips.Contains( xmlName ) )
			{
				return;
			}
			var obj4 = Values[parentNode];
			if ( xmlName == XmlName._CreateContent && obj4 is DataTemplate )
			{
				SetDataTemplate( obj4 as DataTemplate, node );
				return;
			}
			SetPropertyValue( obj4, xmlName, obj, Context.RootElement, node, Context, node );
		}

		static MemberDescriptor DetermineMemberInformation( Type elementType, string namespaceUri, string localname, Assembly assembly, IXmlLineInfo lineInfo )
		{
			var num = localname.IndexOf( '.' );
			if ( num > 0 )
			{
				var name = localname.Substring( 0, num );
				return new MemberDescriptor( Services.Default.GetService<ITypeResolutionService>().GetElementType( new XmlType( new XmlName( namespaceUri, name ) ), lineInfo, assembly ), localname.Substring( num + 1 ) );
			}
			return new MemberDescriptor( elementType, localname );
		}

		public static void SetPropertyValue( object element, XmlName propertyName, object value, object rootElement, INode node, HydratationContext context, IXmlLineInfo lineInfo )
		{
			var serviceProvider = Services.Create( context, node );
			var descriptor = DetermineMemberInformation( element.GetType(), propertyName.NamespaceUri, propertyName.LocalName, context.RootElement.GetType().GetTypeInfo().Assembly, lineInfo );

			var initialized = serviceProvider.GetService<ISupportPropertyInitializeService>().BeginSetProperty( element, descriptor, value );

			var ctx  = new PropertyValueContext( element, initialized, rootElement, lineInfo, descriptor, serviceProvider );
			serviceProvider.GetService<IPropertyValueSetterService>().SetPropertyValue( ctx );

			serviceProvider.GetService<ISupportPropertyInitializeService>().EndSetProperty( element, descriptor, ctx.Value );
		}

		void SetDataTemplate( DataTemplate template, INode node )
		{
			template.LoadTemplate = delegate
			{
				var context = new HydratationContext
				{
					ParentContext = this.Context,
					RootElement = this.Context.RootElement
				};
				Visitor.Apply( context, node );
				return context.Values[node];
			};
		}
	}
}

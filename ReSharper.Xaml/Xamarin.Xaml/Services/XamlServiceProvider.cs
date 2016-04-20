using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public static class ServiceExtensions
	{
		public static TService GetService<TService>( this IServiceProvider @this )
		{
			return (TService)@this.GetService( typeof(TService) );
		}

		public static IServiceRegistry Add<T>( this IServiceRegistry @this, T instance )
		{
			@this.Add( typeof(T), instance );
			return @this;
		}
	}

	public interface IServiceRegistry
	{
		void Add( Type type, object service );
	}

	public class XamlServiceProvider : IServiceProvider, IServiceRegistry
	{
		readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

		public XamlServiceProvider( HydratationContext context = null, INode node = null )
		{
			object targetObject;
			if ( node != null && node.Parent != null && context.Values.TryGetValue( node.Parent, out targetObject ) )
			{
				ProvideValueTarget = new XamlValueTargetProvider( targetObject, node, context, null );
			}
			if ( context != null )
			{
				RootObjectProvider = new XamlRootObjectProvider( context.RootElement );
			}
			if ( node != null )
			{
				XamlTypeResolver = new XamlTypeResolver( node.NamespaceResolver, context.RootElement.GetType().GetTypeInfo().Assembly );
				var node2 = node;
				while ( node2 != null && !( node2 is IElementNode ) )
				{
					node2 = node2.Parent;
				}
				if ( node2 != null )
				{
					NameScopeProvider = new NameScopeProvider
					{
						NameScope = (INameScope)( node2 as IElementNode ).Namescope
					};
				}
			}
			var xmlLineInfo = node as IXmlLineInfo;
			if ( xmlLineInfo != null )
			{
				XmlLineInfoProvider = new XmlLineInfoProvider( xmlLineInfo );
			}
			ValueConverterProvider = new ValueConverterProvider();
		}

		public IProvideValueTarget ProvideValueTarget
		{
			get { return (IProvideValueTarget)GetService( typeof(IProvideValueTarget) ); }
			set { services[typeof(IProvideValueTarget)] = value; }
		}

		public IXamlTypeResolver XamlTypeResolver
		{
			get { return (IXamlTypeResolver)GetService( typeof(IXamlTypeResolver) ); }
			set { services[typeof(IXamlTypeResolver)] = value; }
		}

		public IRootObjectProvider RootObjectProvider
		{
			get { return (IRootObjectProvider)GetService( typeof(IRootObjectProvider) ); }
			set { services[typeof(IRootObjectProvider)] = value; }
		}

		internal IXmlLineInfoProvider XmlLineInfoProvider
		{
			get { return (IXmlLineInfoProvider)GetService( typeof(IXmlLineInfoProvider) ); }
			set { services[typeof(IXmlLineInfoProvider)] = value; }
		}

		internal INameScopeProvider NameScopeProvider
		{
			get { return (INameScopeProvider)GetService( typeof(INameScopeProvider) ); }
			set { services[typeof(INameScopeProvider)] = value; }
		}

		internal IValueConverterProvider ValueConverterProvider
		{
			get { return (IValueConverterProvider)GetService( typeof(IValueConverterProvider) ); }
			set { services[typeof(IValueConverterProvider)] = value; }
		}

		public void Add( Type type, object service )
		{
			services[type] = service;
		}

		public object GetService( Type serviceType )
		{
			object service;
			var result = services.TryGetValue( serviceType, out service ) ? service : null;
			return result;
		}
	}
}

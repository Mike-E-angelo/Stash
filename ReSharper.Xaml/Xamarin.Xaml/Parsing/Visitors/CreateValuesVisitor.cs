using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	public class CreateValuesVisitor : XamlNodeVisitorBase
	{
		public CreateValuesVisitor(HydratationContext context) : base( true )
		{
			this.Context = context;
		}

		IDictionary<INode, object> Values
		{
			get { return Context.Values; }
		}

		HydratationContext Context { get; set; }

		protected override void Visit(ValueNode node, INode parentNode)
		{
			this.Values[node] = node.Value;
		}

		protected override void Visit(ElementNode node, INode parentNode)
		{
			var serviceProvider = Services.Create( Context, node );
			var builder = serviceProvider.GetService<IObjectBuilderService>();
			var type = serviceProvider.GetService<ITypeResolutionService>().GetElementType(node.XmlType, node, Context.RootElement.GetType().GetTypeInfo().Assembly);
			var context = new ObjectBuilderContext( Context, type, node, serviceProvider );
			var instance = builder.Create( context );
			var @checked = CheckForExtension( node, instance, serviceProvider );
			Assign( node, @checked, type );
		}

		object CheckForExtension(IElementNode node, object instance, IServiceProvider provider)
		{
			TypeExtension extension = instance as TypeExtension;
			if (extension != null)
			{
				var visitor = new ApplyPropertiesVisitor( Context );
				foreach ( var current in node.Properties.Values.ToList() )
				{
					current.Accept( visitor, node );
				}
				foreach ( var current2 in node.CollectionItems )
				{
					current2.Accept( visitor, node );
				}
				node.Properties.Clear();
				node.CollectionItems.Clear();
				return extension.ProvideValue( provider );
			}
			return instance;
		}

		void Assign( IElementNode node, object instance, Type type = null )
		{
			var serviceProvider = Services.Create( Context, node );
			Values[node] = instance;
			Context.Types[node] = type ?? instance.GetType();
			var bindableObject = instance as BindableObject;
			if ( bindableObject != null )
			{
				NameScope.SetNameScope(bindableObject, (INameScope)node.Namescope);
			}
	
			serviceProvider.GetService<ISupportInitializeService>().BeginInitialize( instance );
		}

		protected override void Visit(RootNode node, INode parentNode)
		{
			Assign(node, ((RuntimeRootNode)node).Root);
		}

		protected override void Visit(ListNode node, INode parentNode)
		{
			XmlName xmlName;
			if (ApplyPropertiesVisitor.TryGetPropertyName(node, parentNode, out xmlName))
			{
				node.XmlName = xmlName;
			}
		}
	}
}

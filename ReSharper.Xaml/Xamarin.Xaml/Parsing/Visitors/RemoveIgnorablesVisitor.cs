using System;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public class RemoveIgnorablesVisitor : XamlNodeVisitorBase
	{
		readonly HydratationContext context;

		public RemoveIgnorablesVisitor( HydratationContext context ) : base(true)
		{
			this.context = context;
		}

		protected override void Visit( RootNode node, INode parentNode )
		{
			Remove( node, parentNode );
		}

		protected override void Visit( ElementNode node, INode parentNode )
		{
			Remove( node, parentNode );
		}

		protected virtual void Remove( IElementNode node, INode parentNode )
		{
			var ignorable = new XmlName( KnownSchemas.MarkupCompatibility, "Ignorable" );
			var provider = Services.Create( context, node );
			var service = provider.GetService<IProvideParentValues>();
			var nodes = service != null ? service.ParentObjects : new object[0];
			var ignore = nodes.Concat( new[] { node, parentNode } ).Distinct()
				.OfType<IElementNode>()
				.Select( n => n.Properties.ContainsKey( ignorable ) ? n.Properties[ignorable] : null )
				.OfType<ValueNode>()
				.Select( n => n.Value )
				.OfType<string>()
				.SelectMany( s => s.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ) )
				.Distinct()
				.Select( node.NamespaceResolver.LookupNamespace )
				.ToArray();

			var remove = node.Properties.Where( pair => ignore.Contains( pair.Key.NamespaceUri ) ).Select( pair => pair.Key ).ToArray();
			foreach ( var item in remove )
			{
				node.Properties.Remove( item );
			}
		}
	}
}
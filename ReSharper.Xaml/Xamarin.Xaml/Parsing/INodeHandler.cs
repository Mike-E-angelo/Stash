using System;

namespace Xamarin.Xaml
{
	interface INodeHandler
	{
		void Handle( ParsingContext context, INode node );
	}

	abstract class NodeHandler : INodeHandler
	{
		public abstract void Handle( ParsingContext context, INode node );
	}

	class NodeCollectionItemHandler : NodeHandler
	{
		public static NodeCollectionItemHandler Instance
		{
			get { return InstanceField; }
		}

		static readonly NodeCollectionItemHandler InstanceField = new NodeCollectionItemHandler();

		public override void Handle( ParsingContext context, INode node )
		{
			context.Node.CollectionItems.Add( node );
		}
	}

	abstract class PropertyNodeHandler : NodeHandler
	{
		public abstract XmlName GetName( ParsingContext context );

		public override void Handle( ParsingContext context, INode node )
		{
			var name = GetName( context );
			context.Node.Properties.Add( name, node );
		}
	}

	class DeclaredPropertyNodeHandler : PropertyNodeHandler
	{
		readonly XmlName name;

		public DeclaredPropertyNodeHandler( XmlName name )
		{
			this.name = name;
		}

		public override XmlName GetName( ParsingContext context )
		{
			return name;
		}
	}

	class NamedPropertyNodeHandler : PropertyNodeHandler
	{
		public static NamedPropertyNodeHandler Instance
		{
			get { return InstanceField; }
		}

		static readonly NamedPropertyNodeHandler InstanceField = new NamedPropertyNodeHandler();

		public override XmlName GetName( ParsingContext context )
		{
			var name = context.Reader.Name;
			var full = name.StartsWith( context.ElementName + ".", StringComparison.Ordinal ) ? name.Substring( context.ElementName.Length + 1 ) : context.Reader.LocalName;
			var result = new XmlName( context.Reader.NamespaceURI, full );
			return result;
		}
	}
}
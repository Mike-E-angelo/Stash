using System.Xml;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	interface IParsingRule
	{
		bool IsSatisfiedBy( ParsingContext context );
	}

	class ParsingRule : IParsingRule
	{
		readonly XmlNodeType type;

		public ParsingRule( XmlNodeType type )
		{
			this.type = type;
		}

		bool IParsingRule.IsSatisfiedBy( ParsingContext context )
		{
			var result = CanProcess( context );
			return result;
		}

		protected virtual bool CanProcess( ParsingContext context )
		{
			var result = context.Reader.NodeType == Type;
			return result;
		}

		protected XmlNodeType Type
		{
			get { return type; }
		}
	}

	class EndParsingStatement : ParsingStatement
	{
		public static EndParsingStatement Instance
		{
			get { return InstanceField; }
		}	static readonly EndParsingStatement InstanceField = new EndParsingStatement();

		public EndParsingStatement() : base( new ParsingRule( XmlNodeType.EndElement ) )
		{}

		protected override bool ContinueExecution
		{
			get { return false; }
		}
	}

	class ElementRule : ParsingRule
	{
		public static ElementRule Instance
		{
			get { return InstanceField; }
		}	static readonly ElementRule InstanceField = new ElementRule();

		public ElementRule() : base( XmlNodeType.Element )
		{}
	}

	class InnerPropertyStatement : ParsingStatement
	{
		public static InnerPropertyStatement Instance
		{
			get { return InstanceField; }
		}	static readonly InnerPropertyStatement InstanceField = new InnerPropertyStatement();

		public InnerPropertyStatement() : base( ElementRule.Instance, new BuildAndHandleAction( ListNodeBuilder.Instance, NamedPropertyNodeHandler.Instance ) )
		{}

		public override bool IsSatisfiedBy( ParsingContext context )
		{
			var result = base.IsSatisfiedBy( context ) && context.Reader.Name.Contains( "." );
			return result;
		}
	}

	class TypeArgumentsStatement : ParsingStatement
	{
		public static TypeArgumentsStatement Instance
		{
			get { return InstanceField; }
		}	static readonly TypeArgumentsStatement InstanceField = new TypeArgumentsStatement();

		public TypeArgumentsStatement() : base( ElementRule.Instance, new BuildAndHandleAction( ListNodeBuilder.Instance, new DeclaredPropertyNodeHandler( XmlName.xArguments ) ) )
		{}

		public override bool IsSatisfiedBy( ParsingContext context )
		{
			var result = base.IsSatisfiedBy( context ) && context.Reader.NamespaceURI == KnownSchemas.Xaml2009 && context.Reader.LocalName == "Arguments";
			return result;
		}
	}

	class CreateContextStatement : ParsingStatement
	{
		public static CreateContextStatement Instance
		{
			get { return InstanceField; }
		}	static readonly CreateContextStatement InstanceField = new CreateContextStatement();

		public CreateContextStatement() : base( ElementRule.Instance, new BuildAndHandleAction( NodeBuilder.Instance, new DeclaredPropertyNodeHandler( XmlName._CreateContent ) ) )
		{}

		public override bool IsSatisfiedBy( ParsingContext context )
		{
			var result = base.IsSatisfiedBy( context ) && context.Node.XmlType.Name.NamespaceUri == KnownSchemas.Forms2014 && context.Node.XmlType.Name.LocalName == typeof(DataTemplate).Name;
			return result;
		}
	}

	class ElementCollectionItemsStatement : ParsingStatement
	{
		public static ElementCollectionItemsStatement Instance
		{
			get { return InstanceField; }
		}	static readonly ElementCollectionItemsStatement InstanceField = new ElementCollectionItemsStatement();

		public ElementCollectionItemsStatement() : base( ElementRule.Instance, new BuildAndHandleAction( NodeBuilder.Instance, NodeCollectionItemHandler.Instance ) )
		{}
	}

	class TextElementStatement : ParsingStatement
	{
		public static TextElementStatement Instance
		{
			get { return InstanceField; }
		}	static readonly TextElementStatement InstanceField = new TextElementStatement();

		public TextElementStatement() : base( new ParsingRule( XmlNodeType.Text ), new BuildAndHandleAction( TextNodeBuilder.Instance, NodeCollectionItemHandler.Instance ) )
		{}
	}
}
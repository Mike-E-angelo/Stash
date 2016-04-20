using System.Linq;
using System.Xml;

namespace Xamarin.Xaml
{
	public static class KnownSchemas
	{
		public const string 
			Xmlns				= "http://www.w3.org/2000/xmlns/",
			Xaml2006			= "http://schemas.microsoft.com/winfx/2006/xaml",
			Xaml2009			= "http://schemas.microsoft.com/winfx/2009/xaml",
			Forms2014			= "http://xamarin.com/schemas/2014/forms",
			MarkupCompatibility = "http://schemas.openxmlformats.org/markup-compatibility/2006";
	}

	/*interface IXamlParser
	{
		IElementNode Parse( XmlReader reader );
	}*/

	interface IParsingStatement : IParsingRule
	{
		bool Process( ParsingContext context );
	}

	class ParsingContext
	{
		readonly string elementName;
		readonly XmlReader reader;
		readonly IParsingEngine engine;
		readonly IAttributeParser attributeParser;
		readonly IElementNode node;

		public ParsingContext( IParsingEngine engine, IAttributeParser attributeParser, XmlReader reader, IElementNode node )
		{
			this.reader = reader;
			this.engine = engine;
			this.attributeParser = attributeParser;
			this.node = node;
			elementName = reader.Name;
		}

		public string ElementName
		{
			get { return elementName; }
		}

		public XmlReader Reader
		{
			get { return reader; }
		}

		public IParsingEngine Engine
		{
			get { return engine; }
		}

		public IAttributeParser AttributeParser
		{
			get { return attributeParser; }
		}

		public IElementNode Node
		{
			get { return node; }
		}
	}

	class ParsingStatement : IParsingStatement
	{
		readonly IParsingRule rule;
		readonly IParsingAction action;

		protected ParsingStatement( IParsingAction action ) : this( null, action )
		{}

		protected ParsingStatement( IParsingRule rule ) : this( rule, null )
		{}

		public ParsingStatement( IParsingRule rule, IParsingAction action )
		{
			this.rule = rule;
			this.action = action;
		}

		public virtual bool Process( ParsingContext context )
		{
			if ( action != null )
			{
				action.Invoke( context );
			}
			return ContinueExecution;
		}

		protected virtual bool ContinueExecution
		{
			get { return true; }
		}

		public virtual bool IsSatisfiedBy( ParsingContext context )
		{
			var result = rule != null && rule.IsSatisfiedBy( context );
			return result;
		}
	}

	interface IParsingAction
	{
		void Invoke( ParsingContext context );
	}

	class BuildAndHandleAction : IParsingAction
	{
		readonly INodeBuilder builder;
		readonly INodeHandler handler;

		public BuildAndHandleAction( INodeBuilder builder, INodeHandler handler )
		{
			this.builder = builder;
			this.handler = handler;
		}

		public void Invoke( ParsingContext context )
		{
			var node = builder.CreateNode( context );
			handler.Handle( context, node );
		}
	}

	static class ParsingEngineExtensions
	{
		public static IElementNode Run( this IParsingEngine @this, XmlReader reader, object rootObject, IAttributeParser attributeParser = null )
		{
			var context = new ParsingContext( @this, attributeParser ?? AttributeParser.Instance, reader, null );
			var result = new RootNodeBuilder( rootObject ).CreateNode<IElementNode>( context );
			return result;
		}
	}

	class ParsingEngine : IParsingEngine
	{
		readonly IAttributeParser parser;

		public ParsingEngine() : this( AttributeParser.Instance )
		{}

		public ParsingEngine( IAttributeParser parser )
		{
			this.parser = parser;
		}

		protected virtual IParsingStatement[] DetermineStatements( ParsingContext context )
		{
			var result = new IParsingStatement[]
			{
				// Element:
				InnerPropertyStatement.Instance,
				TypeArgumentsStatement.Instance,
				CreateContextStatement.Instance,
				ElementCollectionItemsStatement.Instance, 

				// Text:
				TextElementStatement.Instance, 

				// End (stops processing):
				EndParsingStatement.Instance
			};
			return result;
		}

		public void Run( XmlReader reader, IElementNode node )
		{
			if ( !reader.IsEmptyElement )
			{
				var context = new ParsingContext( this, parser, reader, node );
				var rules = DetermineStatements( context );
				while ( reader.Read() )
				{
					var processor = rules.FirstOrDefault( rule => rule.IsSatisfiedBy( context ) );
					if ( processor != null && !processor.Process( context ) )
					{
						return;
					}
				}
			}
		}
	}
}

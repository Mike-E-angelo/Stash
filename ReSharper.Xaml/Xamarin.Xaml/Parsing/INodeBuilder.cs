using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
    static class NodeBuilderExtensions
    {
        public static T CreateNode<T>( this INodeBuilder @this, ParsingContext context ) where T : IElementNode
        {
            var result = (T)@this.CreateNode( context );
            return result;
        }
    }

    interface INodeBuilder
    {
        INode CreateNode( ParsingContext context );
    }

    class TextNodeBuilder : INodeBuilder
    {
        public static TextNodeBuilder Instance
        {
            get { return InstanceField; }
        }	static readonly TextNodeBuilder InstanceField = new TextNodeBuilder();

        public INode CreateNode( ParsingContext context )
        {
            return new ValueNode( context.Reader.Value.Trim(), (IXmlNamespaceResolver)context.Reader );
        }
    }

    class RootNodeBuilder : ElementNodeBuilderBase
    {
        readonly object root;

        public RootNodeBuilder( object root )
        {
            this.root = root;
        }

        protected override IElementNode Create( string name, XmlReader reader, KeyValuePair<XmlName, INode>[] attributes )
        {
            var result = new RuntimeRootNode( new XmlType( new XmlName(reader.NamespaceURI, reader.Name) ), (IXmlNamespaceResolver)reader, root );
            return result;
        }
    }

    interface IParsingEngine
    {
        void Run( XmlReader reader, IElementNode node );
    }

    abstract class ElementNodeBuilderBase : INodeBuilder
    {
        public INode CreateNode( ParsingContext context )
        {
            var reader = context.Reader;
            var name = reader.Name;
            var attributes = context.AttributeParser.Parse( reader );
            var result = Create( name, reader, attributes );
            result.Properties.AddRange( attributes );
            context.Engine.Run( reader, result );
            return result;
        }

        protected abstract IElementNode Create( string name, XmlReader reader, KeyValuePair<XmlName, INode>[] attributes );
    }

    class ElementNodeBuilder : ElementNodeBuilderBase
    {
        public static ElementNodeBuilder Instance
        {
            get { return InstanceField; }
        }	static readonly ElementNodeBuilder InstanceField = new ElementNodeBuilder();

        protected override IElementNode Create( string name, XmlReader reader, KeyValuePair<XmlName, INode>[] attributes )
        {
            var lineInfo = (IXmlLineInfo)reader;
            var types = attributes.Any( kvp => kvp.Key == XmlName.xTypeArguments ) ?
                ( (ValueNode)attributes.First( kvp => kvp.Key == XmlName.xTypeArguments ).Value ).Value as IList<XmlType>
                : Enumerable.Empty<XmlType>();
            var result = new ElementNode( new XmlType( new XmlName( reader.NamespaceURI, name ), types.ToArray() ), reader as IXmlNamespaceResolver, lineInfo.LineNumber, lineInfo.LinePosition );
            return result;
        }
    }

    class NodeBuilder : INodeBuilder
    {
        public static NodeBuilder Instance
        {
            get { return InstanceField; }
        }	static readonly NodeBuilder InstanceField = new NodeBuilder();

        readonly ElementNodeBuilder builder;

        public NodeBuilder() : this( ElementNodeBuilder.Instance )
        {}

        public NodeBuilder( ElementNodeBuilder builder )
        {
            this.builder = builder;
        }

        public virtual INode CreateNode( ParsingContext context )
        {
            while ( context.Reader.Read() )
            {
                switch ( context.Reader.NodeType )
                {
                    case XmlNodeType.Element:
                        return builder.CreateNode( context );
                }
            }
            var lineInfo = (IXmlLineInfo)context.Reader;
            throw new XamlParseException( "Closing PropertyElement expected", lineInfo );
        }
    }

    class ListNodeBuilder : INodeBuilder
    {
        public static ListNodeBuilder Instance
        {
            get { return InstanceField; }
        }	static readonly ListNodeBuilder InstanceField = new ListNodeBuilder();

        readonly ElementNodeBuilder builder;

        public ListNodeBuilder() : this( ElementNodeBuilder.Instance )
        {}

        public ListNodeBuilder( ElementNodeBuilder builder )
        {
            this.builder = builder;
        }

        public INode CreateNode( ParsingContext context )
        {
            var reader = context.Reader;
            var name = reader.Name;
            var nodes = new List<INode>();
            while ( reader.Read() )
            {
                var nodeType = reader.NodeType;
                var lineInfo = (IXmlLineInfo)reader;
                switch ( nodeType )
                {
                    case XmlNodeType.Element:
                    {
                        var done = reader.IsEmptyElement && reader.Name == name;
                        var element = builder.CreateNode( context );
                        if ( done )
                        {
                            return element;
                        }
                        nodes.Add( element );
                        break;
                    }
                    case XmlNodeType.Text:
                    {
                        var node = new ValueNode( reader.Value.Trim(), (IXmlNamespaceResolver)reader, lineInfo.LineNumber, lineInfo.LinePosition );
                        nodes.Add( node );
                        break;
                    }
                    case XmlNodeType.EndElement:
                        return nodes.Count == 1 ? nodes.Single() : new ListNode( nodes, (IXmlNamespaceResolver)reader, lineInfo.LineNumber, lineInfo.LinePosition );
                }
            }
            throw new XamlParseException( "Closing PropertyElement expected", (IXmlLineInfo)reader );
        }
    }
}
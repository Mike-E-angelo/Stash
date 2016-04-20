using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	public class NamescopingVisitor : XamlNodeVisitorBase
	{
		private readonly Dictionary<INode, INameScope> scopes = new Dictionary<INode, INameScope>();

		public NamescopingVisitor(HydratationContext context) : base( false )
		{
			this.Values = context.Values;
		}
		
		IDictionary<INode, object> Values { get; set; }

		private static bool IsDataTemplate(INode node, INode parentNode)
		{
			IElementNode elementNode = parentNode as IElementNode;
			INode node2;
			return elementNode != null && elementNode.Properties.TryGetValue(XmlName._CreateContent, out node2) && node2 == node;
		}

		protected override void Visit(ValueNode node, INode parentNode)
		{
			this.scopes[node] = this.scopes[parentNode];
		}

		protected override void Visit(MarkupNode node, INode parentNode)
		{
			this.scopes[node] = this.scopes[parentNode];
		}

		protected override void Visit(ElementNode node, INode parentNode)
		{
			INameScope nameScope = (parentNode == null || IsDataTemplate(node, parentNode)) ? new NameScope() : this.scopes[parentNode];
			node.Namescope = nameScope;
			this.scopes[node] = nameScope;
		}

		protected override void Visit(RootNode node, INode parentNode)
		{
			NameScope nameScope = new NameScope();
			node.Namescope = nameScope;
			this.scopes[node] = nameScope;
		}

		protected override void Visit(ListNode node, INode parentNode)
		{
			this.scopes[node] = this.scopes[parentNode];
		}
	}
}

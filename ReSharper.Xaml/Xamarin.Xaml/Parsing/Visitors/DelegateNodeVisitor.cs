using System;

namespace Xamarin.Xaml
{
	public class DelegateNodeVisitor : XamlNodeVisitorBase
	{
		private readonly Action<INode, INode> action;

		public DelegateNodeVisitor(Action<INode, INode> action, bool visitChildrenFirst = false) : base( visitChildrenFirst )
		{
			this.action = action;
		}

		protected override void VisitNode( INode node, INode parentNode )
		{
			this.action(node, parentNode);
		}
	}
}

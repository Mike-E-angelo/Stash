using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	public interface IXamlNodeVisitor
	{
		bool VisitChildrenFirst { get; }

		void Visit(INode node, INode parentNode);
	}

	delegate void VisitNode<in T>( T node, INode parent ) where T : INode;

	public class DelegateList : List<Delegate>
	{
		public Delegate Locate( object item )
		{
			var result = this.FirstOrDefault( @delegate =>
			{
				var typeInfo = @delegate.GetType().GetTypeInfo();
				return typeInfo.IsGenericType && typeInfo.GenericTypeArguments.Any() && typeInfo.GenericTypeArguments.First().IsInstanceOfType(item);
			} );
			return result;
		}
	}

	public abstract class XamlNodeVisitorBase : IXamlNodeVisitor
	{
		readonly bool visitChildrenFirst;
		readonly Lazy<DelegateList> delegates;

		protected XamlNodeVisitorBase( bool visitChildrenFirst )
		{
			this.visitChildrenFirst = visitChildrenFirst;
			delegates = new Lazy<DelegateList>(GetDelegates);
		}

		public virtual bool VisitChildrenFirst
		{
			get {  return visitChildrenFirst; }
		}

		void IXamlNodeVisitor.Visit( INode node, INode parentNode )
		{
			VisitNode( node, parentNode );
		}

		protected virtual void VisitNode( INode node, INode parentNode )
		{
			var action = delegates.Value.Locate(node);
			if ( action != null )
			{
				action.DynamicInvoke( node, parentNode );
			}
		}

		protected virtual DelegateList GetDelegates()
		{
			// TODO: Probably a better way to do this. :)
			var result = new DelegateList
			{
				new VisitNode<RootNode>( Visit ),
				new VisitNode<MarkupNode>( Visit ),
				new VisitNode<ElementNode>( Visit ),
				new VisitNode<ListNode>( Visit ),
				new VisitNode<ValueNode>( Visit ),
			};
			return result;
		}

		protected virtual void Visit(RootNode node, INode parentNode)
		{ }

		protected virtual void Visit(ValueNode node, INode parentNode)
		{ }

		protected virtual void Visit(MarkupNode node, INode parentNode)
		{ }

		protected virtual void Visit(ElementNode node, INode parentNode)
		{ }

		protected virtual void Visit(ListNode node, INode parentNode)
		{ }
	}
}

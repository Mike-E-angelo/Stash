
namespace Xamarin.Xaml
{
	static class Visitor
	{
		public static TNode Apply<TNode>( HydratationContext context, TNode node ) where TNode : INode
		{
			foreach ( var visitor in Services.Create( context, node ).GetService<IVisitorProviderService>().GetVisitors( context, node ) )
			{
				node.Accept( visitor, null );
			}
			return node;
		}
	}
}
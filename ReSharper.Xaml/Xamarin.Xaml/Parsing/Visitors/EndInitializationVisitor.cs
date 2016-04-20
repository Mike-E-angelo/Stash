namespace Xamarin.Xaml
{
	public class EndInitializationVisitor : XamlNodeVisitorBase
	{
		readonly HydratationContext context;

		public EndInitializationVisitor(HydratationContext context) : base( false )
		{
			this.context = context;
		}

		protected override void Visit( RootNode node, INode parentNode )
		{
			End(node);
		}

		protected override void Visit( ElementNode node, INode parentNode )
		{
			End( node );
		}

		void End( INode node )
		{
			var provider = Services.Create( context, node );
			provider.GetService<ISupportInitializeService>().EndInitialize( context.Values[node] );
		}
	}
}
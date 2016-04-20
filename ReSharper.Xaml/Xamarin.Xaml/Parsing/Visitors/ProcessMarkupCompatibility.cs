using System.Linq;

namespace Xamarin.Xaml
{
	public class ProcessMarkupCompatibility : XamlNodeVisitorBase
	{
		public static ProcessMarkupCompatibility Instance
		{
			get { return InstanceField; }
		}	static readonly ProcessMarkupCompatibility InstanceField = new ProcessMarkupCompatibility();

		public ProcessMarkupCompatibility() : base( true )
		{}

		protected override void Visit( RootNode node, INode parentNode )
		{
			Remove( node );
		}

		static void Remove( IElementNode node )
		{
			var remove = node.Properties.Where( pair => pair.Key.NamespaceUri == KnownSchemas.MarkupCompatibility ).Select( pair => pair.Key ).ToArray();
			foreach ( var item in remove )
			{
				node.Properties.Remove( item );
			}
		}

		protected override void Visit( ElementNode node, INode parentNode )
		{
			Remove( node );
		}
	}
}
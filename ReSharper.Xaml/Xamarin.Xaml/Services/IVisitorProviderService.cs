using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Xaml
{
	public interface IVisitorProviderService
	{
		IEnumerable<IXamlNodeVisitor> GetVisitors( HydratationContext context, INode node );
	}

	public class VisitorProviderService : IVisitorProviderService
	{
		public static VisitorProviderService Instance
		{
			get { return InstanceField; }
		}	static readonly VisitorProviderService InstanceField = new VisitorProviderService();

		protected virtual IEnumerable<IXamlNodeVisitor> CreateVisitors( HydratationContext context, INode node )
		{
			return new IXamlNodeVisitor[]
			{
				node is RuntimeRootNode ? new DelegateNodeVisitor( delegate( INode n, INode parent ) { n.Parent = parent; } ) : null,
				new RemoveIgnorablesVisitor( context ),
				ProcessMarkupCompatibility.Instance,
				new ExpandMarkupsVisitor( context ),
				new NamescopingVisitor( context ),
				new CreateValuesVisitor( context ),
				new RegisterXNamesVisitor( context ),
				new FillResourceDictionariesVisitor( context ),
				new ApplyPropertiesVisitor( context ),
				new EndInitializationVisitor( context ),
			};
		}

		public IEnumerable<IXamlNodeVisitor> GetVisitors( HydratationContext context, INode node )
		{
			// TODO: cache:
			var result = CreateVisitors( context, node ).Where( visitor => visitor != null ).ToArray();
			return result;
		}
	}
}
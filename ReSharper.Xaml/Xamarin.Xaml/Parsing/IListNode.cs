using System.Collections.Generic;

namespace Xamarin.Xaml
{
	public interface IListNode : INode
	{
		ICollection<INode> CollectionItems
		{
			get;
		}
	}
}

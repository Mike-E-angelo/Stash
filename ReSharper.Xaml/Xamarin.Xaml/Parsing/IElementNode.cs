using System.Collections.Generic;

namespace Xamarin.Xaml
{
	/*public interface INameScope
	{
		object FindByName(string name);

		void RegisterName(string name, object scopedElement);

		void UnregisterName(string name);
	}

	class NameScopeWrapper : INameScope
	{
		readonly Forms.INameScope inner;

		public NameScopeWrapper( Forms.INameScope inner )
		{
			this.inner = inner;
		}

		public object FindByName( string name )
		{
			return inner.FindByName( name );
		}

		public void RegisterName( string name, object scopedElement )
		{
			inner.RegisterName( name, scopedElement );
		}

		public void UnregisterName( string name )
		{
			inner.UnregisterName( name );
		}
	}*/

	public interface IElementNode : IListNode
	{
		IDictionary<XmlName, INode> Properties { get; }
		
		object Namescope { get; }
		
		XmlType XmlType { get; }
		
		string NamespaceUri { get; }
	}
}

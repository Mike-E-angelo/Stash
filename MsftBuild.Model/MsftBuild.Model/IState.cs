using System.Collections.Generic;
using System.Linq;

namespace MsftBuild.Model
{
	public interface IState
	{
		T Get<T>() where T : class;

		void Set<T>( T value ) where T : class;
	}

	class State : IState
	{
		readonly HashSet<object> store;

		public State() : this( new HashSet<object>() ) {}

		public State( HashSet<object> store )
		{
			this.store = store;
		}

		public T Get<T>() where T : class => store.OfType<T>().FirstOrDefault();

		public void Set<T>( T value ) where T : class => store.Add( value );
	}
}
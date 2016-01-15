using System;

namespace ClassLibrary1
{
	public class Class1 : IDisposable
	{
		public void Dispose()
		{
			// Dispose( true );
			// GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing )
		{}
	}
}

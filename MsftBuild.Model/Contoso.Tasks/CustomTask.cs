using MsftBuild.Model;
using System;

namespace Contoso.Tasks
{
	public class CustomTask : ITask
	{
		public string SomeProperty { get; set; }

		public DateTime SomeDate { get; set; }

		public int SomeNumber { get; set; }

		public void Execute( IServiceProvider provider, IState state )
		{
			Console.WriteLine( "This task simply writes out to the console the specified values." );
			Console.WriteLine( "SomeProperty: {0}", SomeProperty );
			Console.WriteLine( "SomeDate: {0}", SomeDate );
			Console.WriteLine( "SomeNumber: {0}", SomeNumber );
		}
	}
}

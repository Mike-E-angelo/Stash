using PostSharp.Aspects;
using PostSharp.ModuleInitializer.Model;
using System;
using System.Diagnostics;
using Xunit;

namespace PostSharp.ModuleInitializer
{
	public class Class1
	{
		[Fact]
		public void Test()
		{
			if ( !Initializer.Initialized )
			{
				throw new InvalidOperationException( "Should not be here." );
			}
		}
	}

	public class Class2
	{
		[Theory, AutoData]
		public void Test( object parameter )
		{
			if ( !Initializer.Initialized )
			{
				throw new InvalidOperationException( "Should not be here." );
			}
		}
	}
	
	public static class Initializer
	{
		public static bool Initialized { get; private set; }

		[ModuleInitializer( 0 )]
		public static void Initialize()
		{
			Trace.WriteLine( "Initializing" );
			AssemblyInitializer.Instance.Run( typeof(object).Assembly );
			Initialized = true;
		}
	}
}
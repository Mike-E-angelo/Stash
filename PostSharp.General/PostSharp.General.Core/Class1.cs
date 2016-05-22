using PostSharp.Aspects;
using PostSharp.General.Portable;
using System.Diagnostics;

namespace PostSharp.General.Core
{
	public static class Configure
	{
		[ModuleInitializer( 0 ), Runtime( AspectPriority =  0 ), AssemblyInitialize( AspectPriority =  1 )]
		public static void Execute()
		{
			Trace.WriteLine( $"Initializing {typeof(Configure)}" );
		}
	}

	[Validator]
	public class Command : ICommand
	{
		public void Execute() { }
	}
}
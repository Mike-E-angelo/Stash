using PostSharp.Aspects;
using System.Diagnostics;
using System.Reflection;
using PostSharp.General.Portable;

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
	public class Command : ICommand2<MethodBase>
	{
		public virtual void Update() {}

		public virtual bool CanExecute( MethodBase parameter ) => true;

		public void Execute( MethodBase parameter ) { }
	}
}
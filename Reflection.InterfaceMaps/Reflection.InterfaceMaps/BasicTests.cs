using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xunit;

namespace Reflection.InterfaceMaps
{
	public class BasicTests
	{
		[Fact]
		public void VerifyMappings()
		{
			var expected = typeof(Expected).GetTypeInfo().GetRuntimeInterfaceMap( typeof(ICommand) );
			Assert.Equal( "System.Windows.Input.ICommand.Execute", expected.TargetMethods.Last().Name );
			
			var unexpected = typeof(Unexpected).GetTypeInfo().GetRuntimeInterfaceMap( typeof(ICommand) );
			Assert.Equal( "System.Windows.Input.ICommand.Execute", unexpected.TargetMethods.Last().Name ); // Fails with "Execute"
		}
	}

	interface ICommand<in T> : ICommand
	{
		void Execute( T parameter );
	}

	abstract class CommandBase<T> : ICommand<T>
	{
		bool ICommand.CanExecute( object parameter ) => false;
		void ICommand.Execute( object parameter ) {}
		public event EventHandler CanExecuteChanged;

		public abstract void Execute( T parameter );
	}

	class Expected : CommandBase<object>
	{
		public override void Execute( object parameter ) {}
	}

	interface IUnexpected : ICommand<object> {}
	class Unexpected : CommandBase<object>, IUnexpected
	{
		public override void Execute( object parameter ) {}
	}
}

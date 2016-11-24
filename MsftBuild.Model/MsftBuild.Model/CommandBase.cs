using System;
using System.Windows.Input;

namespace MsftBuild.Model
{
	public abstract class CommandBase<T> : ICommand<T>
	{
		public event EventHandler CanExecuteChanged = delegate {};

		bool ICommand.CanExecute( object parameter ) => parameter is T && CanExecute( (T)parameter );
		void ICommand.Execute( object parameter ) => Execute( (T)parameter );

		public virtual bool CanExecute( T parameter ) => !Equals( parameter, default(T) );
		public abstract void Execute( T parameter );
	}
}

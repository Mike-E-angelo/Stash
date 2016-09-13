using PostSharp.Extensibility;
using System;
using System.Windows.Input;

namespace PostSharp.PassThrough
{
	public abstract class ExtensibleCommandBase<T> : ICommand<T>
	{
		public event EventHandler CanExecuteChanged;

		[ExtensionPoint]
		bool ICommand.CanExecute( object parameter ) => parameter is T && IsSatisfiedBy( (T)parameter );

		[ExtensionPoint]
		void ICommand.Execute( object parameter ) {}

		[ExtensionPoint( AttributeInheritance =  MulticastInheritance.Strict )]
		public virtual bool IsSatisfiedBy( T parameter ) => !Equals( parameter, default(T) );

		[ExtensionPoint( AttributeInheritance =  MulticastInheritance.Strict, AttributeTargetMemberAttributes = MulticastAttributes.NonAbstract )]
		public abstract void Execute( T parameter );
	}
}
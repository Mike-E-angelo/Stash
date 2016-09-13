using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Dependencies;
using PostSharp.Aspects.Serialization;
using System;
using System.Windows.Input;

namespace PostSharp.PassThrough
{
	public interface ICommand<in T> : ICommand
	{
		void Execute( T parameter );
	}

	public abstract class CommandBase<T> : ICommand<T>
	{
		public event EventHandler CanExecuteChanged;

		bool ICommand.CanExecute( object parameter ) => parameter is T && IsSatisfiedBy( (T)parameter );

		void ICommand.Execute( object parameter ) {}

		public virtual bool IsSatisfiedBy( T parameter ) => !Equals( parameter, default(T) );

		public abstract void Execute( T parameter );
	}

	[MethodInterceptionAspectConfiguration( SerializerType = typeof(MsilAspectSerializer) )]
	[LinesOfCodeAvoided( 10 ), AttributeUsage( AttributeTargets.Method )]
	[ProvideAspectRole( "Extensibility" )]
	[AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Threading ), 
		AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Caching )]
	public sealed class ExtensionPointAttribute : MethodInterceptionAspect
	{
		public override void OnInvoke( MethodInterceptionArgs args )
		{
			if ( args.Instance is IExtensionAware )
			{
				// Custom logic here ...
				base.OnInvoke( args );
			}
			else
			{
				base.OnInvoke( args );
			}
		}
	}

	public interface IExtensionAware {}
}

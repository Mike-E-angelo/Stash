using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Dependencies;
using PostSharp.Aspects.Serialization;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace ClassLibrary3
{
	[ProvideAspectRole( StandardRoles.Validation ), LinesOfCodeAvoided( 10 ), AttributeUsage( AttributeTargets.Class )]
	[AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Threading ), AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Caching )]
	[MulticastAttributeUsage( Inheritance = MulticastInheritance.Strict, PersistMetaData =  true )]
	public class ApplyAutoValidationAttribute : TypeLevelAspect, IAspectProvider
	{
		/*readonly Func<Type, IEnumerable<AspectInstance>> source;

		public ApplyAutoValidationAttribute() : this( DefaultSource ) {}

		protected ApplyAutoValidationAttribute( Func<Type, IEnumerable<AspectInstance>> source )
		{
			this.source = source;
		}*/

		IEnumerable<AspectInstance> IAspectProvider.ProvideAspects( object targetElement )
		{
			var type = targetElement as Type;
			if ( type != null )
			{
				var interfaces = type.GetTypeInfo().GetRuntimeInterfaceMap( typeof(ICommand) );
				yield return new AspectInstance( interfaces.TargetMethods.First( info => info.Name == "System.Windows.Input.ICommand.CanExecute" ).AccountForGenericDefinition(), new ObjectConstruction( typeof(AutoValidationValidationAspect) ), null );
				// yield return new AspectInstance( interfaces.TargetMethods.First( info => info.Name == "Execute" ), new ObjectConstruction( typeof(AutoValidationExecuteAspect) ), null );
				// yield break;
			}
		}
	}

	public abstract class CommandBase<T> : ICommand
	{
		public event EventHandler CanExecuteChanged = delegate { };
		/*
		readonly Coerce<T> coercer;
		readonly ISpecification<T> specification;

		protected CommandBase() : this( Defaults<T>.Coercer ) {}

		protected CommandBase( [Required] Coerce<T> coercer ) : this( coercer, Specifications<T>.Assigned ) {}

		protected CommandBase( [Required] ISpecification<T> specification ) : this( Defaults<T>.Coercer, specification ) {}

		protected CommandBase( [Required] Coerce<T> coercer, [Required] ISpecification<T> specification )
		{
			this.coercer = coercer;
			this.specification = specification;
		}*/

		// public virtual void Update() => CanExecuteChanged( this, EventArgs.Empty );

		bool ICommand.CanExecute( object parameter ) => true;

		void ICommand.Execute( object parameter ) {}

		/*public virtual bool CanExecute( T parameter ) => specification.IsSatisfiedBy( parameter );

		public abstract void Execute( T parameter );*/
	}

	[MethodInterceptionAspectConfiguration( SerializerType = typeof(MsilAspectSerializer), AspectPriority = 1 )]
	[ProvideAspectRole( StandardRoles.Validation ), LinesOfCodeAvoided( 4 ), AttributeUsage( AttributeTargets.Method )]
	[AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Threading ), AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Caching )]
	public abstract class AutoValidationAspectBase : MethodInterceptionAspect, IInstanceScopedAspect
	{
		/*readonly static Func<Type, ApplyAutoValidationAttribute> Applies = AttributeSupport<ApplyAutoValidationAttribute>.All.Get;

		readonly Func<object, IAspect> factory;
		protected AutoValidationAspectBase( Func<object, IAspect> factory )
		{
			this.factory = factory;
		}*/

		public object CreateInstance( AdviceArgs adviceArgs ) => /*Applies( adviceArgs.Instance.GetType() ) != null ? factory( adviceArgs.Instance ) :*/ new AutoValidationValidationAspect();
		void IInstanceScopedAspect.RuntimeInitializeInstance() {}
	}

	public class AutoValidationValidationAspect : AutoValidationAspectBase
	{
		/*readonly static Func<object, Implementation> Factory = new AspectFactory<Implementation>( controller => new Implementation( controller ) ).Create;
		public AutoValidationValidationAspect() : base( Factory ) {}*/


		sealed class Implementation : AutoValidationValidationAspect
		{
			/*readonly IAutoValidationController controller;
			public Implementation( IAutoValidationController controller )
			{
				this.controller = controller;
			}*/

			public override void OnInvoke( MethodInterceptionArgs args )
			{
				/*var parameter = args.Arguments[0];
				var valid = controller.IsValid( parameter );
				if ( !valid.HasValue )
				{
					controller.MarkValid( parameter, args.GetReturnValue<bool>() );
				}
				else
				{
					args.ReturnValue = valid.Value;
				}*/
			}
		}
	}

	public static class MethodExtensions
	{
		public static MethodInfo AccountForGenericDefinition( this MethodInfo @this )
		{
			var result = @this.DeclaringType.IsConstructedGenericType ? @this.DeclaringType.GetGenericTypeDefinition().GetRuntimeMethods().SingleOrDefault( new MethodEqualitySpecification( @this ).IsSatisfiedBy )
				:
				@this;
			return result;
		}
	}

	public interface ISpecification<in T>
	{
		bool IsSatisfiedBy( T parameter );
	}

	public abstract class SpecificationBase<T> : ISpecification<T>
	{
		public abstract bool IsSatisfiedBy( T parameter );
	}

	public abstract class GuardedSpecificationBase<T> : SpecificationBase<T>
	{
	}

	public abstract class SpecificationWithContextBase<T> : SpecificationWithContextBase<T, T>
	{
		protected SpecificationWithContextBase( T context ) : base( context ) {}
	}

	public abstract class SpecificationWithContextBase<TParameter, TContext> : GuardedSpecificationBase<TParameter>
	{
		protected SpecificationWithContextBase( TContext context )
		{
			Context = context;
		}

		protected TContext Context { get; }
	}

	class MethodEqualitySpecification : SpecificationWithContextBase<MethodInfo>
	{
		readonly Func<Type, Type> map;

		public MethodEqualitySpecification( MethodInfo context ) : base( context )
		{
			map = Map;
		}

		public override bool IsSatisfiedBy( MethodInfo parameter ) =>
			parameter.Name == Context.Name && Map( parameter.ReturnType ) == Context.ReturnType && parameter.GetParameters().Select( info => info.ParameterType ).ToArray().Select( map ).SequenceEqual( Context.GetParameters().Select( info => info.ParameterType ).ToArray() );

		Type Map( Type type )
		{
			var result = type.IsGenericParameter ? Context.DeclaringType.GenericTypeArguments[type.GenericParameterPosition] : type.GetTypeInfo().ContainsGenericParameters ? 
				type.GetGenericTypeDefinition().MakeGenericType( type.GenericTypeArguments.Select( map ).ToArray() ) : type;
			return result;
		}
	}
}

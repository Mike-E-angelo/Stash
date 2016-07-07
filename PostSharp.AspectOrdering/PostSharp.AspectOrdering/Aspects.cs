using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Dependencies;
using PostSharp.Aspects.Serialization;
using System;
using System.Diagnostics;
using System.Reflection;

namespace PostSharp.AspectOrdering
{
	interface IHelloWorld
	{
		void HelloWorld( object context );
	}

	interface IHelloWorld<in T> : IHelloWorld
	{
		void HelloWorld( T context );
	}

	class Class : BaseClass<int>
	{
		[CacheAspect, ValidationAspect]
		public override void HelloWorld( int context ) {}
	}

	abstract class BaseClass<T> : IHelloWorld<T>
	{
		void IHelloWorld.HelloWorld( object context ) => HelloWorld( (T)context );
		public abstract void HelloWorld( T context );
	}

	[MethodInterceptionAspectConfiguration( SerializerType = typeof(MsilAspectSerializer) )]
	[ProvideAspectRole( StandardRoles.Caching ), LinesOfCodeAvoided( 6 ), AttributeUsage( AttributeTargets.Method )]
	class CacheAspect : MethodInterceptionAspect
	{
		public override void OnInvoke( MethodInterceptionArgs args )
		{
			Debug.WriteLine( $"CacheAspect OnInvoke" );
			base.OnInvoke( args );
		}

		public override void RuntimeInitialize( MethodBase method )
		{
			Debug.WriteLine( $"CacheAspect RuntimeInitialize: {method.DeclaringType}.{method}" );
			base.RuntimeInitialize( method );
		}
	}

	[MethodInterceptionAspectConfiguration( SerializerType = typeof(MsilAspectSerializer) )]
	[ProvideAspectRole( StandardRoles.Validation ), LinesOfCodeAvoided( 4 ), AttributeUsage( AttributeTargets.Method )]
	[AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Caching )]
	class ValidationAspect : MethodInterceptionAspect
	{
		public override void RuntimeInitialize( MethodBase method )
		{
			Debug.WriteLine( $"ValidationAspect RuntimeInitialize: {method.DeclaringType}.{method}" );
			base.RuntimeInitialize( method );
		}

		public override void OnInvoke( MethodInterceptionArgs args )
		{
			Debug.WriteLine( $"ValidationAspect OnInvoke" );
			base.OnInvoke( args );
		}
	}

	/*[ProvideAspectRole( StandardRoles.Validation ), LinesOfCodeAvoided( 10 ), AttributeUsage( AttributeTargets.Class )]
	[AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Threading ), AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Caching )]
	[MulticastAttributeUsage( Inheritance = MulticastInheritance.Strict, PersistMetaData = true )]
	class ApplyAttribute : TypeLevelAspect, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects( object targetElement )
		{
			var type = targetElement as Type;
			if ( type != null )
			{
				yield return new ActivateAspectInstanceMethodFactory<ValidationAspect>( typeof(IHelloWorld<>), nameof(IHelloWorld.HelloWorld) ).Create( type );
				yield return new ActivateAspectInstanceMethodFactory<ValidationAspect>( typeof(IHelloWorld), nameof(IHelloWorld.HelloWorld) ).Create( type );
			}
		}
	}*/
}

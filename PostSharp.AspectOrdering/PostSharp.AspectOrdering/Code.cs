using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PostSharp.AspectOrdering
{
	sealed class ActivateAspectInstanceMethodFactory<T> : AspectInstanceFactoryBase where T : IAspect, new()
	{
		public ActivateAspectInstanceMethodFactory( Type implementingType, string methodName ) : base( implementingType, methodName, New.Activate<T>() ) {}
	}

	sealed class ConstructAspectInstanceMethodFactory<T> : AspectInstanceFactoryBase where T : IAspect, new()
	{
		public ConstructAspectInstanceMethodFactory( Type implementingType, string methodName ) : base( implementingType, methodName, New.Construct<T>() ) {}
	}

	static class New
	{
		public static Func<MethodInfo, AspectInstance> Construct<T>( params object[] arguments ) => new ConstructAspectInstanceFactory( new ObjectConstruction( typeof(T), arguments ) ).Create;

		public static Func<MethodInfo, AspectInstance> Activate<T>() where T :  IAspect, new() => new ActivateAspectInstanceFactory( new T() ).Create;
	}

	sealed class ConstructAspectInstanceFactory
	{
		readonly ObjectConstruction construction;

		public ConstructAspectInstanceFactory( ObjectConstruction construction )
		{
			this.construction = construction;
		}

		public AspectInstance Create( MethodInfo parameter ) => new AspectInstance( parameter, construction, null );
	}

	sealed class ActivateAspectInstanceFactory
	{
		readonly IAspect aspect;

		public ActivateAspectInstanceFactory( IAspect aspect )
		{
			this.aspect = aspect;
		}

		public AspectInstance Create( MethodInfo parameter ) => new AspectInstance( parameter, aspect );
	}

	abstract class AspectInstanceFactoryBase
	{
		readonly Type implementingType;
		readonly string methodName;
		readonly Func<MethodInfo, AspectInstance> factory;

		protected AspectInstanceFactoryBase( Type implementingType, string methodName, Func<MethodInfo, AspectInstance> factory )
		{
			this.implementingType = implementingType;
			this.methodName = methodName;
			this.factory = factory;
		}

		public AspectInstance Create( Type parameter )
		{
			var mappings = new MethodMapper( new TypeAdapter( parameter ) ).Create( implementingType );
			var mapping = mappings.SingleOrDefault( pair => pair.InterfaceMethod.Name == methodName && ( pair.MappedMethod.IsFinal || pair.MappedMethod.IsVirtual ) && !pair.MappedMethod.IsAbstract );
			if ( mapping != null )
			{
				var method = mapping.MappedMethod.AccountForGenericDefinition();
				var result = FromMethod( method );
				return result;
			}
			return null;
		}

		AspectInstance FromMethod( MethodInfo method )
		{
			var repository = PostSharpEnvironment.CurrentProject.GetService<IAspectRepositoryService>();
			var instance = factory( method );
			var type = instance.Aspect != null ? instance.Aspect.GetType() : Type.GetType( instance.AspectConstruction.TypeName );
			var result = !repository.HasAspect( method, type ) ? instance : null;
			return result;
		}
	}

	public static class MethodExtensions
	{
		public static MethodInfo AccountForGenericDefinition( this MethodInfo @this )
		{
			var result = @this.DeclaringType.IsConstructedGenericType ? @this.DeclaringType.GetGenericTypeDefinition().GetRuntimeMethods().SingleOrDefault( new MethodEqualitySpecification( @this ).IsSatisfiedBy )
				: @this;
			return result;
		}
	}

	public class TypeAdapter
	{
		public Type Type { get; set; }

		public TypeAdapter( Type type )
		{
			Type = type;
		}

		static IEnumerable<Type> ExpandInterfaces( Type target ) => new[] { target }.Concat( target.GetTypeInfo().ImplementedInterfaces.SelectMany( ExpandInterfaces ) ).Where( x => x.GetTypeInfo().IsInterface ).Distinct();

		public Type[] GetImplementations( Type genericDefinition )
		{
			var result = new[] { Type }.Concat( ExpandInterfaces( Type ) )
									   .Where( type =>
											   {
												   var first = type.GetTypeInfo();
												   var second = genericDefinition.GetTypeInfo();
												   var match = first.IsGenericType && second.IsGenericType && type.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
												   return match;
											   } )
									   .ToArray();
			return result;
		}
	}

	class MethodMapper
	{
		readonly TypeAdapter adapter;

		public MethodMapper( TypeAdapter adapter )
		{
			this.adapter = adapter;
		}

		public MethodMapping[] Create( Type parameter )
		{
			var generic = parameter.GetTypeInfo().IsGenericTypeDefinition ? adapter.GetImplementations( parameter ).FirstOrDefault() : null;
			var implementation = generic ?? ( parameter.IsAssignableFrom( adapter.Type ) ? parameter : null );
			if ( implementation != null )
			{
				var map = adapter.Type.GetTypeInfo().GetRuntimeInterfaceMap( implementation );
				var result = map.InterfaceMethods.Zip( map.TargetMethods, Tuple.Create ).Select( tuple => new MethodMapping( tuple.Item1, tuple.Item2 ) ).ToArray();
				return result;
			}
			return new MethodMapping[0];
		}
	}

	public class MethodMapping
	{
		public MethodMapping( MethodInfo interfaceMethod, MethodInfo mappedMethod )
		{
			InterfaceMethod = interfaceMethod;
			MappedMethod = mappedMethod;
		}

		public MethodInfo InterfaceMethod { get; }
		public MethodInfo MappedMethod { get; }
	}

	class MethodEqualitySpecification : SpecificationWithContextBase<MethodInfo>
	{
		readonly Func<Type, Type> map;

		public MethodEqualitySpecification( MethodInfo context ) : base( context )
		{
			map = Map;
		}

		public override bool IsSatisfiedBy( MethodInfo parameter ) =>
			parameter.Name == Context.Name && Map( parameter.ReturnType ) == Context.ReturnType && parameter.GetParameters().Select( info => info.ParameterType ).Select( map ).SequenceEqual( Context.GetParameters().Select( info => info.ParameterType ) );

		Type Map( Type type )
		{
			var result = type.IsGenericParameter ? Context.DeclaringType.GenericTypeArguments[type.GenericParameterPosition] : type.GetTypeInfo().ContainsGenericParameters ?
				type.GetGenericTypeDefinition().MakeGenericType( type.GenericTypeArguments.Select( map ).ToArray() ) : type;
			return result;
		}
	}

	public abstract class SpecificationWithContextBase<T> : SpecificationWithContextBase<T, T>
	{
		protected SpecificationWithContextBase( T context ) : base( context ) {}
	}

	public abstract class SpecificationWithContextBase<TParameter, TContext> : SpecificationBase<TParameter>
	{
		protected SpecificationWithContextBase( TContext context )
		{
			Context = context;
		}

		protected TContext Context { get; }
	}

	public interface ISpecification
	{
		bool IsSatisfiedBy( object parameter );
	}

	public interface ISpecification<in T> : ISpecification
	{
		bool IsSatisfiedBy( T parameter );
	}

	public abstract class SpecificationBase<T> : ISpecification<T>
	{
		public abstract bool IsSatisfiedBy( T parameter );

		bool ISpecification.IsSatisfiedBy( object parameter ) => IsSatisfiedBy( (T)parameter );
	}
}

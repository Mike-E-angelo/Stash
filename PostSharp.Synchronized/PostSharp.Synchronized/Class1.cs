using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Model.TypeAdapters;
using PostSharp.Patterns.Threading;
using PostSharp.Reflection;
using PostSharp.Serialization;

namespace PostSharp.Synchronized
{
	public static class Extensions
	{
		public static TResult As<TResult>( this object target, Action<TResult> action )
		{
			if ( target is TResult )
			{
				var result = (TResult)target;
				result.With( action );
				return result;
			}
			return default(TResult);
		}
		public static TItem With<TItem>( this TItem @this, Action<TItem> action )
		{
			var result = @this.With( item =>
			{
				action?.Invoke( item );
				return item;
			} );
			return result;
		}

		public static TResult With<TItem, TResult>( this TItem target, Func<TItem, TResult> function, Func<TResult> defaultFunction = null )
		{
			var getDefault = defaultFunction ?? ( () => default(TResult) );
			var result = target != null ? function( target ) : getDefault();
			return result;
		}
	}

	// [Synchronized]
	[Synchronized]
	public class ApplicationFactory : FactoryBase<MethodInfo, object>
	{
		public static ApplicationFactory Instance { get; } = new ApplicationFactory();

		protected override object CreateItem( MethodInfo parameter )
		{
			return null;

			/*using ( new AssignExecutionContextCommand().ExecuteWith( MethodContext.Get( parameter ) ) )
			{
				var autoData = new AutoData( FixtureFactory<OutputCustomization>.Instance.Create(), parameter );
				var application = new LocalAutoDataAttribute.Application( GetType() );
				using ( new ExecuteApplicationCommand( application ).ExecuteWith( autoData ) )
				{
					return application;

					/*var logger = application.Get<ILogger>();
					application.Get<LoggingLevelSwitch>().MinimumLevel = LogEventLevel.Debug;
					logger.Information( "Basic Logging: TestingCommandPerformance" );
					Testing( "Hello World!" );#1#
				}
				// new InitializeOutputCommand( Output ).Run( GetType() );
				/*application.Get<RecordingLogEventSink>().With( PurgingEventFactory.Instance.Create ).Each( Output.WriteLine );#1#
			}*/
		}

		// protected abstract IApplication CreateItem( [Required]MethodInfo parameter );

		
		/*[Export, Shared]
		public class RecordingLoggerFactory : DragonSpark.Diagnostics.RecordingLoggerFactory
		{
			[Export]
			public override LoggingLevelSwitch LevelSwitch => base.LevelSwitch;

			[Export]
			public override RecordingLogEventSink Sink => base.Sink;
		}*/
	}

	public interface IFactory<in TParameter, out TResult> : IFactoryWithParameter
	{
		TResult Create( TParameter parameter );
	}

	public interface IFactoryWithParameter
	{
		object Create( object parameter );

		/*Type ParameterType { get; }*/
	}

	public interface IFactoryParameterCoercer<out TParameter>
	{
		TParameter Coerce( object context );
	}

	public static class FactoryDefaults<T>
	{
		public static ISpecification<T> Always { get; } = AlwaysSpecification.Instance.Wrap<T>();

		public static IFactoryParameterCoercer<T> Coercer { get; } = new FixedFactoryParameterCoercer<T>();
	}

	public class AlwaysSpecification : FixedSpecification
	{
		public static AlwaysSpecification Instance { get; } = new AlwaysSpecification();

		AlwaysSpecification() : base( true )
		{}
	}

	public static class SpecificationExtensions
	{
		public static ISpecification<T> Inverse<T>( this ISpecification @this ) => @this.Inverse().Wrap<T>();

		public static ISpecification Inverse( this ISpecification @this ) => new InverseSpecification( @this );

		public static ISpecification<T> Wrap<T>( this ISpecification @this ) => new DecoratedSpecification<T>( @this );
	}

	public class DecoratedSpecification<T> : SpecificationBase<T>
	{
		readonly ISpecification inner;
		readonly Func<T, object> transform;

		public DecoratedSpecification( [Required]ISpecification inner  ) : this( inner, t => t ) {}

		public DecoratedSpecification( [Required]ISpecification inner, [Required]Func<T, object> transform )
		{
			this.inner = inner;
			this.transform = transform;
		}

		protected override bool Verify( T parameter ) => inner.IsSatisfiedBy( transform( parameter ) );
	}

	public class InverseSpecification : ISpecification
	{
		readonly ISpecification inner;

		public InverseSpecification( [Required] ISpecification inner )
		{
			this.inner = inner;
		}

		public bool IsSatisfiedBy( object context ) => !inner.IsSatisfiedBy( context );
	}

	public class FixedSpecification : ISpecification
	{
		readonly bool satisfied;

		public FixedSpecification( bool satisfied )
		{
			this.satisfied = satisfied;
		}

		public bool IsSatisfiedBy( object context ) => satisfied;
	}

	public class FixedFactoryParameterCoercer<TParameter> : IFactoryParameterCoercer<TParameter>
	{
		public static FixedFactoryParameterCoercer<TParameter> Null { get; } = new FixedFactoryParameterCoercer<TParameter>();

		readonly TParameter item;

		public FixedFactoryParameterCoercer() : this( default(TParameter) )
		{}

		public FixedFactoryParameterCoercer( TParameter item )
		{
			this.item = item;
		}

		public TParameter Coerce( object context ) => item;
	}

	public interface IFactory
	{
		object Create();
	}

	public interface IFactory<out T> : IFactory
	{
		new T Create();
	}

	public static class TypeExtensions
	{
		public static Type GetMemberType(this MemberInfo memberInfo)
		{
		  if (memberInfo is MethodInfo)
			return ((MethodInfo) memberInfo).ReturnType;
		  if (memberInfo is PropertyInfo)
			return ((PropertyInfo) memberInfo).PropertyType;
		  if (memberInfo is FieldInfo)
			return ((FieldInfo) memberInfo).FieldType;
		  return null;
		}

		// public static Type Initialized( this Type @this ) => TypeInitializer.Instance.Create( @this );

		public static Assembly[] Assemblies( [Required] this IEnumerable<Type> @this ) => @this.Select( x => x.Assembly() ).Distinct().ToArray();

		public static TypeAdapter Adapt( [Required]this Type @this ) => new TypeAdapter( @this );

		public static TypeAdapter Adapt( this object @this ) => @this.GetType().Adapt();

		public static TypeAdapter Adapt( [Required]this TypeInfo @this ) => Adapt( @this.AsType() );

		public static Assembly Assembly( [Required]this Type @this ) => Adapt( @this ).Assembly;
	}

	public abstract class FactoryBase<TResult> : IFactory<TResult>
	{
		readonly ISpecification specification;

		protected FactoryBase() : this( AlwaysSpecification.Instance ) {}

		protected FactoryBase( [Required]ISpecification specification )
		{
			this.specification = specification;
		}

		protected abstract TResult CreateItem();

		public virtual TResult Create() => specification.IsSatisfiedBy( this ) ? CreateItem() : default(TResult);

		object IFactory.Create() => Create();
	}

	public abstract class FactoryBase<TParameter, TResult> : IFactory<TParameter, TResult>
	{
		readonly ISpecification<TParameter> specification;
		readonly IFactoryParameterCoercer<TParameter> coercer;

		protected FactoryBase() : this( FactoryDefaults<TParameter>.Coercer ) {}

		protected FactoryBase( [Required]IFactoryParameterCoercer<TParameter> coercer ) : this( FactoryDefaults<TParameter>.Always, coercer ) {}

		protected FactoryBase( [Required]ISpecification<TParameter> specification ) : this( specification, FactoryDefaults<TParameter>.Coercer ) {}

		protected FactoryBase( [Required]ISpecification<TParameter> specification, [Required]IFactoryParameterCoercer<TParameter> coercer )
		{
			this.specification = specification;
			this.coercer = coercer;
		}

		protected abstract TResult CreateItem( [Required]TParameter parameter );

		public TResult Create( TParameter parameter ) => specification.IsSatisfiedBy( parameter ) ? CreateItem( parameter ) : default(TResult);

		object IFactoryWithParameter.Create( object parameter )
		{
			var qualified = coercer.Coerce( parameter );
			var result = Create( qualified );
			return result;
		}
	}

	public static class Attributes
	{
		/*sealed class Cached<T> : AssociatedValue<IAttributeProvider> where T : AttributeProviderFactoryBase
		{
			public Cached( object instance ) : this( instance, Services.Get<T>() ) {}

			public Cached( object instance, T factory ) : base( instance, factory.GetHashCode().ToString(), () => factory.Create( instance ) ) {}
		}*/

		public static IAttributeProvider Get( [Required]object target ) => AttributeProviderFactory.Instance.Create( target );

		public static IAttributeProvider Get( [Required]MemberInfo target, bool includeRelated ) => includeRelated ? GetWithRelated( target ) : Get( target );

		public static IAttributeProvider GetWithRelated( [Required]object target ) => ExpandedAttributeProviderFactory.Instance.Create( target );
	}

	public interface ITypeDefinitionProvider
	{
		/*var result = type.FromMetadata<MetadataTypeAttribute, Type>( item => item.MetadataClassType );
		return result;*/
		TypeInfo GetDefinition( TypeInfo info );
	}

	public static class ObjectExtensions
	{
		public static T ConvertTo<T>( this object @this ) => @this.With( x => (T)ConvertTo( @this, typeof( T ) ) );

		public static object ConvertTo( this object @this, Type to )
		{
			var info = to.GetTypeInfo();
			return !info.IsAssignableFrom( @this.GetType().GetTypeInfo() ) ? ( info.IsEnum ? Enum.Parse( to, @this.ToString() ) : ChangeType( @this, to ) ) : @this;
		}

		static object ChangeType( object @this, Type to )
		{
			try
			{
				return Convert.ChangeType( @this, to );
			}
			catch ( InvalidCastException )
			{
				return null;
			}
		}

		public static IEnumerable<TItem> NotNull<TItem>( this IEnumerable<TItem> target ) => NotNull( target, t => t );

		public static IEnumerable<TItem> NotNull<TItem, TCheck>( this IEnumerable<TItem> target, Func<TItem, TCheck> check ) => target.Where( x => !check( x ).IsNull() );

		public static bool IsNull<T>( this T @this ) => Equals( @this, null );

		public static U FirstWhere<T, U>( this IEnumerable<T> @this, Func<T, U> where ) => @this.NotNull().Select( @where ).NotNull().FirstOrDefault();

		public static TResult Loop<TItem,TResult>( this TItem current, Func<TItem,TItem> resolveParent, Func<TItem, bool> condition, Func<TItem, TResult> extract = null, TResult defaultValue = default(TResult) )
		{
			do
			{
				if ( condition( current ) )
				{
					var result = extract( current );
					return result;
				}
				current = resolveParent( current );
			}
			while ( current != null );
			return defaultValue;
		}

		public static TResult AsTo<TSource, TResult>( this object target, Func<TSource,TResult> transform, Func<TResult> resolve = null )
		{
			var @default = resolve ?? ( () => default(TResult) );
			var result = target is TSource ? transform( (TSource)target ) : @default();
			return result;
		}
	}
	public class TypeAdapter
	{
		public TypeAdapter( [Required]System.Type type ) : this( type.GetTypeInfo() )
		{
			Type = type;
		}

		public TypeAdapter( [Required]TypeInfo info )
		{
			Info = info;
		}

		public System.Type Type { get; }

		public TypeInfo Info { get; }

		// readonly static MethodInfo LambdaMethod = typeof(Expression).GetRuntimeMethods().First(x => x.Name == nameof(Expression.Lambda) && x.GetParameters()[1].ParameterType == typeof(ParameterExpression[]));
// static readonly MethodInfo EqualsMethod = typeof (object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public);

		// Attribution: https://weblogs.asp.net/ricardoperes/detecting-default-values-of-value-types
		/*bool IsDefaultUsingLinqAndDynamic()
		{
			var arguments = new Expression[] { Expression.Default( type ) };
			var paramExpression = Expression.Parameter(type, "x");
			var equalsMethod = type.GetRuntimeMethod( nameof(Equals), new [] { type } );
			var call = Expression.Call(paramExpression, equalsMethod, arguments);
			var lambdaArgType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
			var lambdaMethod = LambdaMethod.MakeGenericMethod(lambdaArgType);
			var expression = lambdaMethod.Invoke(null, new object[] { call, new[] { paramExpression } }) as LambdaExpression;
 
			//cache this, please
			var func = expression.Compile();
			dynamic arg = obj;
			dynamic del = func;
			var result = del( arg );
			return result;
		}*/

		public bool IsDefined<T>( [Required] bool inherited = false ) where T : Attribute => Info.IsDefined( typeof(T), inherited );
		
		public object GetDefaultValue() => Info.IsValueType && Nullable.GetUnderlyingType( Type ) == null ? Activator.CreateInstance( Type ) : null;

		public ConstructorInfo FindConstructor( params object[] parameters ) => FindConstructor( parameters.Select( o => o?.GetType() ).ToArray() );

		public ConstructorInfo FindConstructor( params System.Type[] parameterTypes ) => Info.DeclaredConstructors.SingleOrDefault( c => c.IsPublic && !c.IsStatic && Match( c.GetParameters(), parameterTypes ) );

		static bool Match( IReadOnlyCollection<ParameterInfo> parameters, IReadOnlyCollection<System.Type> provided )
		{
			var result = 
				provided.Count >= parameters.Count( info => !info.IsOptional ) && 
				provided.Count <= parameters.Count && 
				parameters.Select( ( t, i ) => provided.ElementAtOrDefault( i ).With( t.ParameterType.Adapt().IsAssignableFrom, () => i < provided.Count || t.IsOptional ) ).All( b => b );
			return result;
		}

		public object Qualify( object instance ) => instance.With( o => Info.IsAssignableFrom( o.GetType().GetTypeInfo() ) ? o : /*GetCaster( o.GetType() ).With( caster => caster.Invoke( null, new[] { o } ) ) )*/ null );

		public bool IsAssignableFrom( TypeInfo other ) => IsAssignableFrom( other.AsType() );

		public bool IsAssignableFrom( Type other ) => Info.IsAssignableFrom( other.GetTypeInfo() ) /*|| GetCaster( other ) != null*/;

		public bool IsInstanceOfType( object context ) => context.With( o => IsAssignableFrom( o.GetType() ) );

		// MethodInfo GetCaster( System.Type other ) => null; // Info.DeclaredMethods.SingleOrDefault( method => method.Name == "op_Implicit" && method.GetParameters().First().ParameterType.GetTypeInfo().IsAssignableFrom( other.GetTypeInfo() ) );

		public Assembly Assembly => Info.Assembly;

		public System.Type[] GetHierarchy( bool includeRoot = true )
		{
			var result = new List<System.Type> { Type };
			var current = Info.BaseType;
			while ( current != null )
			{
				if ( current != typeof(object) || includeRoot )
				{
					result.Add( current );
				}
				current = current.GetTypeInfo().BaseType;
			}
			return result.ToArray();
		}

		public System.Type GetEnumerableType() => InnerType( Type, types => types.FirstOrDefault(), i => i.Adapt().IsGenericOf( typeof(IEnumerable<>) ) );

		// public Type GetResultType() => type.Append( ExpandInterfaces( type ) ).FirstWhere( t => InnerType( t, types => types.LastOrDefault() ) );

		static System.Type InnerType( System.Type target, Func<System.Type[], System.Type> fromGenerics, Func<TypeInfo, bool> check = null )
		{
			var info = target.GetTypeInfo();
			var result = info.IsGenericType && info.GenericTypeArguments.Any() && check.With( func => func( info ), () => true ) ? fromGenerics( info.GenericTypeArguments ) :
				target.IsArray ? target.GetElementType() : null;
			return result;
		}

		public bool IsGenericOf<T>( bool includeInterfaces = true ) => IsGenericOf( typeof(T).GetGenericTypeDefinition(), includeInterfaces );

		public bool IsGenericOf( System.Type genericDefinition, bool includeInterfaces = true ) =>
			new Type[] {Type}.Concat( includeInterfaces ? ExpandInterfaces( Type ) : new Type[0] ).Select( type => type.GetTypeInfo() ).Any( typeInfo => typeInfo.IsGenericType && genericDefinition.GetTypeInfo().IsGenericType && typeInfo.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition() );

		public System.Type[] GetAllInterfaces() => ExpandInterfaces( Type ).ToArray();

		static IEnumerable<System.Type> ExpandInterfaces( System.Type target ) => new Type[] {target}.Concat( target.GetTypeInfo().ImplementedInterfaces.SelectMany( ExpandInterfaces ) ).Where( x => x.GetTypeInfo().IsInterface ).Distinct();

		public System.Type[] GetEntireHierarchy() => ExpandInterfaces( Type ).Union( GetHierarchy( false ) ).Distinct().ToArray();
	}

	public class TypeDefinitionProvider : ITypeDefinitionProvider
	{
		public static TypeDefinitionProvider Instance { get; } = new TypeDefinitionProvider();

		class Context
		{
			readonly TypeInfo current;
			readonly Lazy<TypeInfo> metadata;

			public Context( TypeInfo current )
			{
				this.current = current;
				metadata = new Lazy<TypeInfo>( ResolveMetadata );
			}

			public Context CreateFromBaseType() => current.BaseType.With( x => new Context( x.GetTypeInfo() ) );

			public TypeInfo Metadata => metadata.Value;

			TypeInfo ResolveMetadata() => Type.GetType( $"{current.FullName}Metadata, {current.Assembly.FullName}", false )?.GetTypeInfo();
		}

		public TypeInfo GetDefinition( TypeInfo info )
		{
			var context = new Context( info );
			var result = context.Loop( 
				item => item.CreateFromBaseType(), 
				item => item.Metadata != null,
				item => item.Metadata
				);
			return result;
		}
	}

	public class MemberInfoLocator : TransformerBase<MemberInfo>, IMemberInfoLocator
	{
		public static MemberInfoLocator Instance { get; } = new MemberInfoLocator( TypeDefinitionProvider.Instance );

		readonly ITypeDefinitionProvider provider;

		public MemberInfoLocator( ITypeDefinitionProvider provider )
		{
			this.provider = provider;
		}

		static PropertyInfo GetDeclaredProperty( MemberInfo info, TypeInfo x )
		{
			try
			{
				return x.GetDeclaredProperty( info.Name );
			}
			catch ( AmbiguousMatchException )
			{
				var result = x.DeclaredProperties.FirstOrDefault( propertyInfo => propertyInfo.Name == info.Name );
				return result;
			}
		}

		protected override MemberInfo CreateItem( MemberInfo parameter )
		{
			var typeInfo = parameter as TypeInfo;
			var key = typeInfo ?? parameter.DeclaringType.GetTypeInfo();
			var result = provider.GetDefinition( key ).With( x => typeInfo != null ? x : parameter.AsTo<PropertyInfo, MemberInfo>( y => GetDeclaredProperty( parameter, x ) ) );
			return result;
		}
	}

	public abstract class TransformerBase<T> : FactoryBase<T, T>, ITransformer<T>
	{
		protected TransformerBase() {}

		protected TransformerBase( [Required]ISpecification<T> specification  ) : base( specification ) {}
	}

	public class CodeContainer<T>
	{
		readonly Lazy<int> value;

		public CodeContainer( [Required] params object[] items ) : this( KeyFactory.Instance.Create, items ) {}

		public CodeContainer( Func<IEnumerable<object>, int> factory, [Required] params object[] items )
		{
			var all = new []{typeof(T)}.Concat( items ).ToArray();
			value = new Lazy<int>( () => factory( all ) );
		}

		public int Code => value.Value;
	}

	public abstract class KeyFactory<T> : FactoryBase<IEnumerable<object>, T>
	{
		public T CreateUsing( params object[] parameter ) => Create( parameter );
	}

	/*public class MemberInfoTransformer : Factory<MemberInfo, int>
	{
		public static MemberInfoTransformer Instance { get; } = new MemberInfoTransformer();

		public MemberInfoTransformer() : base( IsTypeSpecification<MemberInfo>.Instance ) {}

		protected override int CreateItem( MemberInfo parameter ) => parameter is TypeInfo
			? 
			parameter.GetHashCode() : 
			parameter.DeclaringType.GetTypeInfo().GUID.GetHashCode() * 6776 + parameter.ToString().GetHashCode();
	}

	public class ParameterInfoTransformer : Factory<ParameterInfo, int>
	{
		public static ParameterInfoTransformer Instance { get; } = new ParameterInfoTransformer();

		public ParameterInfoTransformer() : base( IsTypeSpecification<ParameterInfo>.Instance ) {}

		protected override int CreateItem( ParameterInfo parameter ) => 
			parameter.Member.DeclaringType.GetTypeInfo().GUID.GetHashCode() * 6776 + parameter.ToString().GetHashCode();
	}*/

	public class KeyFactory : KeyFactory<int>
	{
		public static KeyFactory Instance { get; } = new KeyFactory();

		protected override int CreateItem( IEnumerable<object> parameter )
		{
			var result = 0x2D2816FE;
			foreach ( var o in parameter )
			{
				var next = result * 31;
				var increment = GetCode( o );
				result += next + increment;
			}
			return result;
		}

		int GetCode( object o )
		{
			var text = o as string;
			if ( text != null )
			{
				return text.GetHashCode();
			}

			var items = o as IEnumerable;
			if ( items != null )
			{
				return Create( items.Cast<object>() );
			}

			/*if ( o is MemberInfo )
			{
				return MemberInfoTransformer.Instance.Create( o as MemberInfo );
			}

			if ( o is ParameterInfo )
			{
				return ParameterInfoTransformer.Instance.Create( o as ParameterInfo );
			}*/
			
			return o?.GetHashCode() ?? 0;
		}
	}

	public class MemberInfoAttributeProviderFactory : FactoryBase<MemberInfoAttributeProviderFactory.Parameter, IAttributeProvider>
	{
		public static MemberInfoAttributeProviderFactory Instance { get; } = new MemberInfoAttributeProviderFactory( MemberInfoLocator.Instance );

		public class Parameter
		{
			readonly CodeContainer<Parameter> container;

			public Parameter( [Required] MemberInfo member, bool inherit )
			{
				Member = member;
				Inherit = inherit;
				container = new CodeContainer<Parameter>( member, inherit );
			}

			public MemberInfo Member { get; }
			public bool Inherit { get; }

			public override int GetHashCode() => container.Code;
		}

		readonly IMemberInfoLocator locator;

		public MemberInfoAttributeProviderFactory( [Required]IMemberInfoLocator locator )
		{
			this.locator = locator;
		}

		protected override IAttributeProvider CreateItem( Parameter parameter ) => new MemberInfoAttributeProvider( locator.Create( parameter.Member ) ?? parameter.Member, parameter.Inherit );
	}

	public interface IMemberInfoLocator : ITransformer<MemberInfo>
	{}

	public interface ITransformer<T> : IFactory<T, T>
	{}

	class MemberInfoProviderFactory : MemberInfoProviderFactoryBase
	{
		public static MemberInfoProviderFactory Instance { get; } = new MemberInfoProviderFactory( MemberInfoAttributeProviderFactory.Instance );

		public MemberInfoProviderFactory( MemberInfoAttributeProviderFactory inner ) : base( inner, false ) {}
	}

	abstract class MemberInfoProviderFactoryBase : FactoryBase<object, IAttributeProvider>
	{
		readonly MemberInfoAttributeProviderFactory inner;
		readonly bool includeRelated;

		protected MemberInfoProviderFactoryBase( [Required]MemberInfoAttributeProviderFactory inner, bool includeRelated )
		{
			this.inner = inner;
			this.includeRelated = includeRelated;
		}

		protected override IAttributeProvider CreateItem( object parameter )
		{
			var item = new MemberInfoAttributeProviderFactory.Parameter( parameter as MemberInfo ?? ( parameter as Type ?? parameter.GetType() ).GetTypeInfo(), includeRelated );
			var result = inner.Create( item );
			return result;
		}
	}

	class ExpandedAttributeProviderFactory : AttributeProviderFactoryBase
	{
		public static ExpandedAttributeProviderFactory Instance { get; } = new ExpandedAttributeProviderFactory( MemberInfoWithRelatedProviderFactory.Instance );

		public ExpandedAttributeProviderFactory( MemberInfoWithRelatedProviderFactory factory ) : base( factory ) {}
	}

	class MemberInfoWithRelatedProviderFactory : MemberInfoProviderFactoryBase
	{
		public static MemberInfoWithRelatedProviderFactory Instance { get; } = new MemberInfoWithRelatedProviderFactory( MemberInfoAttributeProviderFactory.Instance );

		// public MemberInfoWithRelatedProviderFactory() : this(  ) {}

		public MemberInfoWithRelatedProviderFactory( MemberInfoAttributeProviderFactory inner ) : base( inner, true ) {}
	}

	class AttributeProviderFactory : AttributeProviderFactoryBase
	{
		public static AttributeProviderFactory Instance { get; } = new AttributeProviderFactory( MemberInfoProviderFactory.Instance );
		
		public AttributeProviderFactory( MemberInfoProviderFactory factory ) : base( factory ) {}
	}

	abstract class AttributeProviderFactoryBase : FirstFromParameterFactory<object, IAttributeProvider>
	{
		protected AttributeProviderFactoryBase( MemberInfoProviderFactoryBase factory ) : base( IsAssemblyFactory.Instance.Create, factory.Create ) {}

		class IsAssemblyFactory : DecoratedFactory<object, IAttributeProvider>
		{
			public static IsAssemblyFactory Instance { get; } = new IsAssemblyFactory();

			IsAssemblyFactory() : base( IsTypeSpecification<Assembly>.Instance, o => new AssemblyAttributeProvider( (Assembly)o ) ) {}
		}
	}

	public class DecoratedFactory<T, U> : FactoryBase<T, U>
	{
		readonly Func<T, U> inner;

		public DecoratedFactory( Func<T, U> inner ) : this( FactoryDefaults<T>.Always, inner ) {}

		public DecoratedFactory( [Required]ISpecification<T> specification, [Required]Func<T, U> inner ) : base( specification )
		{
			this.inner = inner;
		}

		protected override U CreateItem( T parameter ) => inner( parameter );
	}

	public class IsTypeSpecification<T> : SpecificationBase<object>
	{
		public static IsTypeSpecification<T> Instance { get; } = new IsTypeSpecification<T>();

		protected override bool Verify( object parameter ) => parameter is T;
	}

	public class FirstFromParameterFactory<T, U> : FactoryBase<T, U>
	{
		readonly IEnumerable<Func<T, U>> inner;

		public FirstFromParameterFactory( params IFactory<T, U>[] factories ) : this( factories.Select( factory => new Func<T, U>( factory.Create ) ).ToArray() ) {}

		public FirstFromParameterFactory( [Required]params Func<T, U>[] inner ) : this( FactoryDefaults<T>.Coercer, inner ) {}

		public FirstFromParameterFactory( IFactoryParameterCoercer<T> coercer, [Required]params Func<T, U>[] inner ) : base( coercer )
		{
			this.inner = inner;
		}

		protected override U CreateItem( T parameter ) => inner.FirstWhere( factory => factory( parameter ) );

		// protected virtual U DetermineFirst( IEnumerable<Func<T, U>> factories, T parameter ) => factories.FirstWhere( factory => factory( parameter ) );
	}

	public interface IAttributeProvider
	{
		bool Contains( System.Type attribute );

		Attribute[] GetAttributes( [Required]System.Type attributeType );
	}

	public class MemberInfoAttributeProvider : AttributeProviderBase
	{
		public MemberInfoAttributeProvider( [Required]MemberInfo info, bool inherit ) : base( type => info.IsDefined( type, inherit ), type => info.GetCustomAttributes<Attribute>( inherit ) ) {}
	}

	public class AssemblyAttributeProvider : AttributeProviderBase
	{
		public AssemblyAttributeProvider( [Required]Assembly assembly ) : base( assembly.IsDefined, assembly.GetCustomAttributes ) {}
	}

	public abstract class AttributeProviderBase : IAttributeProvider
	{
		readonly Func<System.Type, bool> defined;
		readonly Func<System.Type, IEnumerable<Attribute>> factory;

		protected AttributeProviderBase( [Required]Func<System.Type, bool> defined, [Required]Func<System.Type, IEnumerable<Attribute>> factory )
		{
			this.defined = defined;
			this.factory = factory;
		}

		public bool Contains( Type attribute ) => defined( attribute );

		public Attribute[] GetAttributes( Type attributeType ) => defined( attributeType ) ? factory( attributeType ).ToArray() : new Attribute[0];
	}

	public static class AttributeProviderExtensions
	{
		/*public static TResult From<TAttribute, TResult>( this object @this, Func<TAttribute, TResult> resolveValue, Func<TResult> resolveDefault = null ) where TAttribute : Attribute => From( Attributes.Get( @this ), resolveValue, resolveDefault );

		public static TResult From<TAttribute, TResult>( [Required]this IAttributeProvider @this, Func<TAttribute, TResult> resolveValue, Func<TResult> resolveDefault = null ) where TAttribute : Attribute => @this.GetAttributes<TAttribute>().WithFirst( resolveValue, resolveDefault );*/

		public static bool Has<TAttribute>( this object @this ) where TAttribute : Attribute => Has<TAttribute>( Attributes.Get( @this ) );

		public static bool Has<TAttribute>( [Required]this IAttributeProvider @this ) where TAttribute : Attribute => @this.Contains( typeof(TAttribute) );

		public static TAttribute GetAttribute<TAttribute>( this object @this ) where TAttribute : Attribute => GetAttribute<TAttribute>( Attributes.Get( @this ) );

		public static TAttribute GetAttribute<TAttribute>( [Required]this IAttributeProvider @this ) where TAttribute : Attribute => @this.GetAttributes<TAttribute>().FirstOrDefault();

		public static TAttribute[] GetAttributes<TAttribute>( this object @this ) where TAttribute : Attribute => GetAttributes<TAttribute>( Attributes.Get( @this ) );

		public static TAttribute[] GetAttributes<TAttribute>( [Required] this IAttributeProvider @this ) where TAttribute : Attribute => @this.GetAttributes( typeof(TAttribute) ).Cast<TAttribute>().ToArray();
	}

	public interface ISpecification
	{
		bool IsSatisfiedBy( object context );
	}

	public interface ISpecification<in TContext> : ISpecification
	{
		bool IsSatisfiedBy( TContext parameter );
	}

	public abstract class SpecificationBase<TParameter> : ISpecification<TParameter>
	{
		bool ISpecification.IsSatisfiedBy( object parameter ) => parameter is TParameter && IsSatisfiedBy( (TParameter)parameter );

		public bool IsSatisfiedBy( TParameter parameter ) => Verify( parameter );

		protected abstract bool Verify( TParameter parameter );
	}

	public class DefaultValuePropertySpecification : SpecificationBase<PropertyInfo>
	{
		public static DefaultValuePropertySpecification Instance { get; } = new DefaultValuePropertySpecification();

		protected override bool Verify( PropertyInfo parameter ) => parameter.Has<DefaultValueAttribute>() || parameter.Has<DefaultValueBase>();
	}

	[AttributeUsage( AttributeTargets.Property )]
	public abstract class DefaultValueBase : HostingAttribute
	{
		protected DefaultValueBase( Func<object, IDefaultValueProvider> provider ) : base( provider ) {}
	}

	public interface IDefaultValueProvider
	{
		object GetValue( DefaultValueParameter parameter );
	}

	public class DefaultValueParameter
	{
		public DefaultValueParameter( [Required]object instance, [Required]PropertyInfo metadata )
		{
			Instance = instance;
			Metadata = metadata;
		}

		public object Instance { get; }

		public PropertyInfo Metadata { get; }

		public DefaultValueParameter Assign( object value )
		{
			Metadata.SetValue( Instance, value );
			return this;
		}
	}

	public abstract class HostingAttribute : Attribute, IFactoryWithParameter
	{
		readonly Func<object, object> inner;

		protected HostingAttribute( [Required]Func<object, object> inner )
		{
			this.inner = inner;
		}

		public bool CanCreate( object parameter ) => true;

		public object Create( object parameter ) => inner( parameter );
	}

	[MulticastAttributeUsage( MulticastTargets.Property, PersistMetaData = false ), AspectRoleDependency( AspectDependencyAction.Order, AspectDependencyPosition.Before, StandardRoles.Validation )]
	[PSerializable, ProvideAspectRole( "Default Object Values" ), LinesOfCodeAvoided( 6 )]
	[AttributeUsage( AttributeTargets.Assembly ), ReaderWriterSynchronized]
	public sealed class ApplyDefaultValues : LocationInterceptionAspect, IInstanceScopedAspect
	{
		public override bool CompileTimeValidate( LocationInfo locationInfo ) => DefaultValuePropertySpecification.Instance.IsSatisfiedBy( locationInfo.PropertyInfo );

		[Writer]
		public override void OnGetValue( LocationInterceptionArgs args )
		{
			Debugger.Break();

			/*var apply = new Checked( this ).Item.Apply();
			if ( apply )
			{
				var parameter = new DefaultValueParameter( args.Instance ?? args.Location.DeclaringType, args.Location.PropertyInfo );
				var value = DefaultPropertyValueFactory.Instance.Create( parameter );
				args.SetNewValue( args.Value = value );
			}
			else
			{
				base.OnGetValue( args );
			}*/
		}

		public override void OnSetValue( LocationInterceptionArgs args )
		{
			// new Checked( this ).Item.Apply();
			base.OnSetValue( args );
		}

		object IInstanceScopedAspect.CreateInstance( AdviceArgs adviceArgs ) => MemberwiseClone();

		void IInstanceScopedAspect.RuntimeInitializeInstance() {}
	}

	public class FrameworkConfiguration
	{
		public static FrameworkConfiguration Current { get; private set; } = new FrameworkConfiguration();

		public static void Initialize( FrameworkConfiguration configuration ) => Current = configuration;

		public Diagnostics Diagnostics { get; set; } = new Diagnostics();
	}

	class DefaultValueProvider : IDefaultValueProvider
	{
		readonly Func<object> value;

		public DefaultValueProvider( object value ) : this( () => value ) {}

		public DefaultValueProvider( Func<object> value )
		{
			this.value = value;
		}

		public virtual object GetValue( DefaultValueParameter parameter ) => value()?.ConvertTo( parameter.Metadata.PropertyType );
	}

	public class DefaultAttribute : DefaultValueBase
	{
		public DefaultAttribute( object value ) : base( t => new DefaultValueProvider( value ) ) {}

		public DefaultAttribute( Func<object> factory ) : base( t => new DefaultValueProvider( factory ) ) {}
	}

	public class Diagnostics
	{
		[Default( typeof(Diagnostics) )]
		public Type ProfilerFactoryType { get; set; }

		/*[Default( LogEventLevel.Information )]
		public LogEventLevel Level { get; set; }*/
	}
}

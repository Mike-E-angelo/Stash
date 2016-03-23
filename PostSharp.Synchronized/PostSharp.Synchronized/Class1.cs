using System;
using System.Reflection;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Threading;

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

	public interface ISpecification<in TContext> : ISpecification
	{
		bool IsSatisfiedBy( TContext parameter );
	}

	public interface ISpecification
	{
		bool IsSatisfiedBy( object context );
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

	public abstract class SpecificationBase<TParameter> : ISpecification<TParameter>
	{
		bool ISpecification.IsSatisfiedBy( object parameter ) => parameter is TParameter && IsSatisfiedBy( (TParameter)parameter );

		public bool IsSatisfiedBy( TParameter parameter ) => Verify( parameter );

		protected abstract bool Verify( TParameter parameter );
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
}

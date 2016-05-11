using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Serialization;
using PostSharp.Extensibility;
using System;

namespace PostSharp.Breakpoints.External
{
	public partial class Configure
	{
		[ModuleInitializer( 0 ), Runtime]
		public static void Execute()
		{
			new Testing().Run(); // Place a breakpoint here.
		}

		class Testing
		{
			public void Run()
			{} // It should break here.
		}
	}

	public sealed class Runtime : SpecificationBasedAspect
	{
		public Runtime() : base( RuntimeSpecification.Instance ) {}
	}

	public class RuntimeSpecification : SpecificationBase<object>
	{
		public static RuntimeSpecification Instance { get; } = new RuntimeSpecification();

		public override bool IsSatisfiedBy( object parameter ) => !PostSharpEnvironment.IsPostSharpRunning;
	}

	[LinesOfCodeAvoided( 4 )]
	[MethodInterceptionAspectConfiguration( SerializerType = typeof(MsilAspectSerializer) )]
	public abstract class SpecificationBasedAspect : MethodInterceptionAspect
	{
		readonly ISpecification specification;

		protected SpecificationBasedAspect( ISpecification specification )
		{
			this.specification = specification;
		}

		public sealed override void OnInvoke( MethodInterceptionArgs args )
		{
			if ( specification.IsSatisfiedBy( this ) )
			{
				base.OnInvoke( args );
			}
		}
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
		readonly Func<object, T> coercer;

		protected SpecificationBase() : this( o => o is T ? (T)o : default(T) ) {}

		protected SpecificationBase( Func<object, T> coercer )
		{
			this.coercer = coercer;
		}

		public abstract bool IsSatisfiedBy( T parameter );

		bool ISpecification.IsSatisfiedBy( object parameter ) => IsSatisfiedByCoerced( coercer( parameter ) );

		protected virtual bool IsSatisfiedByCoerced( T parameter ) => IsSatisfiedBy( parameter );
	}
}

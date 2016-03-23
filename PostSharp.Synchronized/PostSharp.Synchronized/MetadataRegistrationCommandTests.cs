using PostSharp.Patterns.Model;
using PostSharp.Synchronized;
using System;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace DragonSpark.Testing.Setup.Registration
{
	public class MetadataRegistrationCommandTests : Tests
	{
		public MetadataRegistrationCommandTests( ITestOutputHelper output ) : base( output ) {}

		[Fact]
		public void First() => MethodBase.GetCurrentMethod().As<MethodInfo>( methodUnderTest =>
		{
			Output.WriteLine( "Hello World." );

			ApplicationFactory.Instance.Create( methodUnderTest );
		} );

		[Fact]
		public void Second() => MethodBase.GetCurrentMethod().As<MethodInfo>( methodUnderTest =>
		{
			Output.WriteLine( "Hello World." );

			ApplicationFactory.Instance.Create( methodUnderTest );
		} );
	}

	[Disposable]
	public abstract class Tests
	{
		protected Tests( ITestOutputHelper output ) : this( output, type => output.WriteLine( "Initialized!" ) ) {}

		protected Tests( ITestOutputHelper output, Action<Type> initialize )
		{
			Output = output;
			initialize( GetType() );
		}

		[Reference]
		protected ITestOutputHelper Output { get; }

		protected virtual void Dispose( bool disposing ) {}
	}
}
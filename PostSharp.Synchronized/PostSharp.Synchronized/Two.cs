using System;
using System.Reflection;
using PostSharp.Patterns.Model;
using Xunit;
using Xunit.Abstractions;

namespace PostSharp.Synchronized
{
	public class Two : Tests
	{
		public Two( ITestOutputHelper output ) : base( output ) {}

		[Fact]
		public void First()
		{
			var temp = FrameworkConfiguration.Current.Diagnostics.ProfilerFactoryType;
			Assert.NotNull( temp );
		}

		[Fact]
		public void Second()
		{
			var temp = FrameworkConfiguration.Current.Diagnostics.ProfilerFactoryType;
			Assert.NotNull( temp );
		}
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
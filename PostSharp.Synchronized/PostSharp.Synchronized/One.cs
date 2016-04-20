using Xunit;
using Xunit.Abstractions;

namespace PostSharp.Synchronized
{
	public class One : Tests
	{
		public One( ITestOutputHelper output ) : base( output ) {}

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
}

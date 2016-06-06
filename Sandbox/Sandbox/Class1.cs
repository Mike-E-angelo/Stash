using JetBrains.dotMemoryUnit;
using Xunit;
using Xunit.Abstractions;

namespace Sandbox
{
	public class Class1
	{
		public Class1( ITestOutputHelper output )
		{
			DotMemoryUnitTestOutput.SetOutputMethod( output.WriteLine );
		}

		[Fact]
		[DotMemoryUnit( SavingStrategy = SavingStrategy.OnCheckFail, Directory = @"C:\dotMemory", FailIfRunWithoutSupport = false )]
		[AssertTraffic( AllocatedObjectsCount = 0 )]
		public void BasicTest()
		{
			
		}

		[Fact]
		[DotMemoryUnit( SavingStrategy = SavingStrategy.OnCheckFail, Directory = @"C:\dotMemory", FailIfRunWithoutSupport = false )]
		[AssertTraffic( AllocatedObjectsCount = 0 )]
		public void BasicTest2()
		{
			
		}
	}
}

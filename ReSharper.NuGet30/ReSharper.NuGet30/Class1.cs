using Xunit;

namespace ReSharper.NuGet30
{
	public class Class1
	{
		[Theory, Ploeh.AutoFixture.Xunit2.AutoData]
		public void TestingMethod()
		{
			Assert.True( true );
		}
	}
}

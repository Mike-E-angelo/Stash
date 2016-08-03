using Xunit;

namespace ReSharper.NuGet30
{
	public class Class1
	{
		[Theory, Ploeh.AutoFixture.Xunit2.AutoData]
		public void TestingMethod( int sut )
		{
			Assert.True( true );
		}

		[Theory]
		[InlineData( 3 )]
		[InlineData( 5 )]
		[InlineData( 6 )]
		public void MyFirstTheory( int value )
		{
			Assert.True( IsOdd( value ) );
		}

		bool IsOdd( int value )
		{
			return value % 2 == 1;
		}

		[Fact]
		public void Testing() {}
	}
}

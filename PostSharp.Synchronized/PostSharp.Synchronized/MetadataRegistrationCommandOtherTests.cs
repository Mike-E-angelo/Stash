using PostSharp.Synchronized;
using System.Reflection;
using Xunit;

namespace DragonSpark.Testing.Setup.Registration
{
	public class MetadataRegistrationCommandOtherTests
	{
		[Fact]
		public void FirstOther() => MethodBase.GetCurrentMethod().As<MethodInfo>( methodUnderTest =>
		{
			ApplicationFactory.Instance.Create( methodUnderTest );

		} );

		[Fact]
		public void SecondOther() => MethodBase.GetCurrentMethod().As<MethodInfo>( methodUnderTest =>
		{
			ApplicationFactory.Instance.Create( methodUnderTest );
		} );
	}

	
}

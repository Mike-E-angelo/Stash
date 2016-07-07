using System.Diagnostics;
using Xunit;

namespace PostSharp.AspectOrdering
{
	public class Tests
	{
		[Fact]
		public void BasicTest()
		{
			var temp = new Class();
			temp.HelloWorld( 123 );
			Debugger.Break();
		}
	}
}

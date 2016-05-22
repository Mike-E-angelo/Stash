using PostSharp.General.Core;
using Xunit;

namespace PostSharp.General
{
	public class ReSharperCtFreeze
	{
		[Fact]
		public void Run()
		{
			new Command().Execute();
		}
	}
}

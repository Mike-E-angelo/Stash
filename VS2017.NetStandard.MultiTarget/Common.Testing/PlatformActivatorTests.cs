using LightInject;
using System.Linq;
using Xunit;

namespace Common.Testing
{
	public class PlatformActivatorTests
	{
		[Fact]
		public void Count()
		{
			var container = new ServiceContainer();
			container.Register<IAssemblyProvider, AssemblyProvider>();
			container.Register<Subject>();

			var sut = container.GetInstance<Subject>();
			var assemblies = sut.Get()
			                    .ToArray();
			Assert.NotEmpty(assemblies);
		}
	}
}
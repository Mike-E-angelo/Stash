using DragonSpark.Compose;
using DragonSpark.Composition;
using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrutor.Decorate.Model;

namespace Scrutor.Decorate;

public class UnitTest1
{
	[Fact]
	public void Test1()
	{
		var options = ContainerOptions.Default.Clone().WithMicrosoftSettings().WithAspNetCoreSettings();
		var root    = new ServiceContainer(options);
		var services = new ServiceCollection().Start<IProcess>()
		                                      .Forward<Process>()
		                                      .Decorate<StatusAwareProcess>()
		                                      .Decorate<CancelAwareProcess>()
		                                      .Include(x => x.Dependencies.Recursive())
		                                      .Scoped()
		                                      .Then;

		root.CreateServiceProvider(services);
	}
}


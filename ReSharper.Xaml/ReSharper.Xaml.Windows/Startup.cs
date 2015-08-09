using System.Windows;

namespace ReSharper.Xaml.Windows
{
	public class Startup<TApplication> where TApplication : Application, new()
	{
		public void Run()
		{
			var application = new TApplication();
			application.Run();
		}
	}
}
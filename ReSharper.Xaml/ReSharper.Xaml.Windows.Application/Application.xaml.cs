using System.Windows;
using Xamarin.Xaml;

namespace ReSharper.Xaml.Windows.Application
{
	public partial class Application : System.Windows.Application
	{
		public Application()
		{
			this.BuildUpFromResources();
			// InitializeComponent();
		}

		protected override void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );
			MainWindow.Show();
		}
	}
}

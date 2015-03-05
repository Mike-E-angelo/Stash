using System;
using System.Windows;
using App2;

namespace WpfApplication1
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnStartup( StartupEventArgs e )
		{
			var working = new AWorkingContentPage();
			var breaks = new AContentPage();

			base.OnStartup( e );
		}

		
	}
}

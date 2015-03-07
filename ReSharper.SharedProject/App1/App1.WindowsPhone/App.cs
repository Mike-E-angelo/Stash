
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace App1
{
	partial class App
	{
		private TransitionCollection transitions;
		/// <summary>
		/// Restores the content transitions after the app has launched.
		/// </summary>
		/// <param name="sender">The object where the handler is attached.</param>
		/// <param name="e">Details about the navigation event.</param>
		private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
		{
			var rootFrame = sender as Frame;
			rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
			rootFrame.Navigated -= this.RootFrame_FirstNavigated;
		}

		partial void OnInitializeFrame( Frame rootFrame )
		{
			// Removes the turnstile navigation for startup.
			if (rootFrame.ContentTransitions != null)
			{
				this.transitions = new TransitionCollection();
				foreach (var c in rootFrame.ContentTransitions)
				{
					this.transitions.Add(c);
				}
			}

			rootFrame.ContentTransitions = null;
			rootFrame.Navigated += this.RootFrame_FirstNavigated;
		}
	}
}

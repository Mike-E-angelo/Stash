using System.Windows.Input;

namespace MsftBuild.Model
{
	public interface ICommand<in T> : ICommand
	{
		bool CanExecute( T parameter );
		void Execute( T parameter );
	}
}
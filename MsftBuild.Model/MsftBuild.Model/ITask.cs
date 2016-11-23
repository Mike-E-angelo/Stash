using System;

namespace MsftBuild.Model
{
	public interface ITask
	{
		void Execute( IServiceProvider provider, IState state );
	}
}
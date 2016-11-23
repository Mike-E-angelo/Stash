using System.Collections.Generic;

namespace MsftBuild.Model.ApplicationModel
{
	public interface IArgumentParser
	{
		IDictionary<string, string> Parse( string[] arguments );
	}
}
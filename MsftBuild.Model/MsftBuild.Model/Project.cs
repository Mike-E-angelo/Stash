using System;
using System.Linq;

namespace MsftBuild.Model
{
	public class Project : System.Collections.Generic.List<object>, IBuildInput
	{
		public object GetService( Type serviceType ) => this.First( serviceType.IsInstanceOfType );
	}
}
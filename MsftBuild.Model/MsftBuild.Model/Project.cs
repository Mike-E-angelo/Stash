using System;
using System.Collections.Generic;
using System.Linq;

namespace MsftBuild.Model
{
	public class Project : List<object>, IServiceProvider
	{
		public object GetService( Type serviceType ) => this.First( serviceType.IsInstanceOfType );
	}
}
using PostSharp.Patterns.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace PostSharp.ModuleInitializer.Model
{
	/*public static class Initializer
	{
		[ModuleInitializer( 0 )]
		public static void Initialize()
		{
			
		}
	}*/

	public class AutoDataAttribute : DataAttribute
	{
		public override IEnumerable<object[]> GetData( MethodInfo testMethod )
		{
			AssemblyInitializer.Instance.Run( testMethod.DeclaringType.Assembly );
			return new List<object[]> { new [] { new object() } };
		}
	}


	[Synchronized]
	public class AssemblyInitializer //: CommandBase<Assembly>
	{
		public static AssemblyInitializer Instance { get; } = new AssemblyInitializer();

		AssemblyInitializer() /*: base( Specification.Instance )*/ {}

		public void Run( Assembly parameter )
		{
			// Thread.Sleep( 2000 );
			parameter.GetModules().Select( module => module.ModuleHandle ).ToList().ForEach( System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor );
			/*if ( !Activated( parameter ) )
			{
				parameter.Set( DragonSpark.TypeSystem.Activated.Property, true );
			}*/
		}

		// static bool Activated( Assembly parameter ) => parameter.Get( DragonSpark.TypeSystem.Activated.Property );

		/*class Specification : OncePerParameterSpecification<Assembly>
		{
			public static Specification Instance { get; } = new Specification();

			public override bool IsSatisfiedBy( Assembly parameter ) => !Activated( parameter ) && base.IsSatisfiedBy( parameter );
		}*/
	}
}

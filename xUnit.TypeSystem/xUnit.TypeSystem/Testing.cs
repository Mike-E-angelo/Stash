using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace xUnit.TypeSystem
{
	public class OneClass
	{
		readonly ITestOutputHelper output;

		public OneClass( ITestOutputHelper output )
		{
			this.output = output;
		}

		[Fact]
		public void OutputHashCode()
		{
			Support.Add( typeof(SampleObject).GetTypeInfo() );
			output.WriteLine( "Initialized:" );
			Support.Output( output );

			Support.Add( typeof(SampleObject).GetTypeInfo() );
			output.WriteLine( "After Initialized:" );
			Support.Output( output );
		}
	}

	public class AnotherClass
	{
		readonly ITestOutputHelper output;

		public AnotherClass( ITestOutputHelper output )
		{
			this.output = output;
		}

		[Fact]
		public void OutputHashCode()
		{
			Support.Add( typeof(SampleObject).GetTypeInfo() );
			output.WriteLine( "Initialized:" );
			Support.Output( output );

			Support.Add( typeof(SampleObject).GetTypeInfo() );
			output.WriteLine( "After Initialized:" );
			Support.Output( output );
		}
	}

	public static class Support
	{
		readonly static ICollection<int> Numbers = new List<int>();

		public static void Add( TypeInfo info )
		{
			var code = info.DeclaredConstructors.Single().GetParameters().Single().GetHashCode();
			Numbers.Add( code );
		}

		public static void Output( ITestOutputHelper output )
		{
			foreach ( var number in Numbers.ToArray() )
			{
				output.WriteLine( number.ToString() );
			}
		}
	}

	public class SampleObject
	{
		public SampleObject( object parameter ) {}
	}
}

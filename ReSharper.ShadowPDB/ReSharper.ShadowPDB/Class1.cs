using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace ReSharper.ShadowPDB
{
	public class Class1
	{
		[Fact]
		public void Testing()
		{
			var builder = new ConventionBuilder();
			var attributes = builder.GetCustomAttributes( typeof(SomeClass), typeof(SomeClass).GetConstructor( new[] { typeof(object) } ).GetParameters().Single() );
			Assert.NotEmpty( attributes );
		}

		public class SomeClass
		{
			public SomeClass( [Optional]object parameter )
			{
			}
		}

		public class ConventionBuilder : System.Composition.Convention.ConventionBuilder
		{
			public override IEnumerable<Attribute> GetCustomAttributes( Type reflectedType, ParameterInfo parameter )
			{
				return base.GetCustomAttributes( reflectedType, parameter );
			}
		}
	}
}

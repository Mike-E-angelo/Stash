using PostSharp.Patterns.Contracts;
using System;
using System.Reflection;
using Xunit;

namespace PostSharp.General
{
	public class Class1
	{
		[Fact]
		public void Throws()
		{
			Assert.Throws<ArgumentNullException>( () => { new TypeAdapter( null ); } );
		}

		public class TypeAdapter
		{
			public TypeAdapter( [Required] Type type ) : this( type, type.GetTypeInfo() ) {}

			public TypeAdapter( [Required] TypeInfo info ) : this( info.AsType(), info ) {}

			public TypeAdapter( [Required] Type type, [Required] TypeInfo info )
			{
				Type = type;
				Info = info;
			}

			public Type Type { get; }

			public TypeInfo Info { get; }
		}
	}
}

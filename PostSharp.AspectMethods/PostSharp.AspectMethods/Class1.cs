using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PostSharp.AspectMethods
{
	public class AspectMethodTests
	{
		[Fact]
		public void Methods()
		{
			var instance = (IHelloWorld)new HelloWorld();
			var message = instance.Hello( "Some Message" );
			Assert.Equal( "Hello world! Reflected Type: PostSharp.AspectMethods.HelloWorld, PostSharp.AspectMethods, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null.  Message: Some Message", message );
		}
	}

	[LinesOfCodeAvoided( 1 )]
	class ApplyAttribute : Attribute, IAspectProvider
	{
		public IEnumerable<AspectInstance> ProvideAspects( object targetElement )
		{
			var type = targetElement as Type;
			if ( type != null )
			{
				var map = type.GetTypeInfo().GetRuntimeInterfaceMap( typeof(IHelloWorld) );
				var method = map.InterfaceMethods.Zip( map.TargetMethods, Tuple.Create ).Where( tuple => tuple.Item1.Name == nameof(IHelloWorld.Hello) ).Select( tuple => tuple.Item2 ).Single();
				var result = new[] { new AspectInstance( method, new MethodAspect( method.ReflectedType ) ) };
				return result;
			}
			return Enumerable.Empty<AspectInstance>();
		}
	}

	public interface IHelloWorld
	{
		string Hello( string message );
	}

	[LinesOfCodeAvoided( 1 )]
	[Serializable]
	class MethodAspect : MethodInterceptionAspect
	{
		public Type ReflectedType { get; set; }

		public MethodAspect( Type reflectedType )
		{
			ReflectedType = reflectedType;
		}

		public override void OnInvoke( MethodInterceptionArgs args )
		{
			args.ReturnValue = ( $"Hello world! Reflected Type: {ReflectedType.AssemblyQualifiedName}.  Message: {args.Arguments.Single()}" );
		}
	}

	[Apply]
	class HelloWorld : HelloWorldBase {}

	abstract class HelloWorldBase : IHelloWorld
	{
		string IHelloWorld.Hello( string message )
		{
			return null;
		}
	}
}

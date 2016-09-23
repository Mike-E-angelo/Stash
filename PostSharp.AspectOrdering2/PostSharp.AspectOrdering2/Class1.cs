using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Extensibility;
using System;
using System.Threading;
using Xunit;

namespace PostSharp.AspectOrdering2
{
	public class Class1
	{
		[Fact]
		public void Verify()
		{
			var sut = new Subject();
		}
	}

	[Serializable, LinesOfCodeAvoided( 1 )]
	class FirstAspect : TypeLevelAspect
	{
		public override void CompileTimeInitialize( Type type, AspectInfo aspectInfo )
		{
			MessageSource.MessageSink.Write( new Message( MessageLocation.Unknown, SeverityType.ImportantInfo, "6776", $"{this}: CompileTimeInitialize", null, null, null ));
			Thread.Sleep( 250 );
			base.CompileTimeInitialize( type, aspectInfo );
		}

		protected override void SetAspectConfiguration( AspectConfiguration aspectConfiguration, Type targetType )
		{
			base.SetAspectConfiguration( aspectConfiguration, targetType );
			MessageSource.MessageSink.Write( new Message( MessageLocation.Unknown, SeverityType.ImportantInfo, "6776", $"{this}: SetAspectConfiguration", null, null, null ));
			Thread.Sleep( 250 );
		}
	}

	[Serializable, LinesOfCodeAvoided( 1 )/*, AspectTypeDependency( AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(FirstAspect) )*/]
	class SecondAspect : TypeLevelAspect
	{
		public override void CompileTimeInitialize( Type type, AspectInfo aspectInfo )
		{
			MessageSource.MessageSink.Write( new Message( MessageLocation.Unknown, SeverityType.ImportantInfo, "6776", $"{this}: CompileTimeInitialize", null, null, null ));
			Thread.Sleep( 250 );
			base.CompileTimeInitialize( type, aspectInfo );
		}

		protected override void SetAspectConfiguration( AspectConfiguration aspectConfiguration, Type targetType )
		{
			base.SetAspectConfiguration( aspectConfiguration, targetType );
			MessageSource.MessageSink.Write( new Message( MessageLocation.Unknown, SeverityType.ImportantInfo, "6776", $"{this}: SetAspectConfiguration", null, null, null ));
			Thread.Sleep( 250 );
		}
	}

	[SecondAspect, FirstAspect]
	class Subject {}
}

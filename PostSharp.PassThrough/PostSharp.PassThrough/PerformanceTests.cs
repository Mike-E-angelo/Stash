using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Serialization;
using System;
using Xunit;
using Xunit.Abstractions;

namespace PostSharp.PassThrough
{
	public class PerformanceTests
	{
		readonly static object Parameter = new object();

		readonly Action<string> output;

		public PerformanceTests( ITestOutputHelper helper )
		{
			output = helper.WriteLine;
		}

		[Fact]
		public void RunCommands()
		{
			new PerformanceSupport( output, RunBasicCoreCommand, RunAspectCommand, RunExtensibleCommand, RunEnabledExtensibleCommand ).Run();
		}

		static void RunBasicCoreCommand() => BasicCoreCommand.Default.Execute( Parameter );
		static void RunAspectCommand() => AspectCommand.Default.Execute( Parameter );
		static void RunExtensibleCommand() => ExtensibleCommand.Default.Execute( Parameter );
		static void RunEnabledExtensibleCommand() => EnabledExtensibleCommand.Default.Execute( Parameter );

		class BasicCoreCommand : CommandBase<object>
		{
			public static BasicCoreCommand Default { get; } = new BasicCoreCommand();
			BasicCoreCommand() {}

			public override void Execute( object parameter ) {}
		}

		[MethodInterceptionAspectConfiguration( SerializerType = typeof(MsilAspectSerializer) )]
		[LinesOfCodeAvoided( 1 ), AttributeUsage( AttributeTargets.Method )]
		public sealed class EmptyAspect : MethodInterceptionAspect {}

		class AspectCommand : CommandBase<object>
		{
			public static AspectCommand Default { get; } = new AspectCommand();
			AspectCommand() {}

			[EmptyAspect]
			public override void Execute( object parameter ) {}
		}

		class ExtensibleCommand : ExtensibleCommandBase<object>
		{
			public static ExtensibleCommand Default { get; } = new ExtensibleCommand();
			ExtensibleCommand() {}

			public override void Execute( object parameter ) {}
		}

		[EnableExtensions]
		class EnabledExtensibleCommand : ExtensibleCommandBase<object>
		{
			public static EnabledExtensibleCommand Default { get; } = new EnabledExtensibleCommand();
			EnabledExtensibleCommand() {}

			public override void Execute( object parameter ) {}
		}

		[IntroduceInterface( typeof(IExtensionAware) )]
		[AttributeUsage( AttributeTargets.Class )]
		[AspectConfiguration( SerializerType = typeof(MsilAspectSerializer) )]
		public class EnableExtensionsAttribute : InstanceLevelAspect, IExtensionAware {}
	}
}

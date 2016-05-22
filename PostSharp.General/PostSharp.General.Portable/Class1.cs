using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace PostSharp.General.Portable
{
	public interface ICommand
	{
		void Execute();
	}

	[PSerializable]
	public sealed class Validator : TypeLevelAspect
	{
		[OnMethodInvokeAdvice, MulticastPointcut( MemberName = "Execute", Attributes = MulticastAttributes.Instance )]
		public void OnExecute( MethodInterceptionArgs args ) {}
	}

	[PSerializable, LinesOfCodeAvoided( 4 )]
	public sealed class AssemblyInitializeAttribute : OnMethodBoundaryAspect {}

	[PSerializable, LinesOfCodeAvoided( 4 )]
	public sealed class Runtime : MethodInterceptionAspect {}
}

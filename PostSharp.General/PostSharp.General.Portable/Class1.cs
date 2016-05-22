using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace PostSharp.General.Portable
{
	public interface ICommand2<in TParameter> // : ICommand
	{
		bool CanExecute( TParameter parameter );

		void Execute( TParameter parameter );

		void Update();
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

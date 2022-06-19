using DragonSpark.Model.Selection.Conditions;

namespace Scrutor.Decorate.Model;

sealed class ShouldAvoidStatus : Condition<Exception>
{
	public static ShouldAvoidStatus Default { get; } = new();

	ShouldAvoidStatus() : base(_ => false) {}
}
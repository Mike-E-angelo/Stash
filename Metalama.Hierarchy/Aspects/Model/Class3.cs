namespace Aspects.Model;

public sealed class Registrations : ICommand<IServiceCollection>
{
	public static Registrations Default { get; } = new();

	Registrations() {}
	public void Execute(IServiceCollection parameter)
	{
		parameter.AddSingleton<ILogException>(LogException.Default)
		         .AddSingleton<IExceptionLogger, ExceptionLogger>()
		         .AddSingleton<IExceptions, Exceptions>()
		         .AddSingleton<IApplicationErrorHandler, ApplicationErrorHandler>()
		         .AddSingleton<ILastChanceExceptionHandler, LastChanceExceptionHandler>();
	}
}

public sealed class EventToCommandBehavior : CommunityToolkit.Maui.Behaviors.EventToCommandBehavior
{
	protected override void OnAttachedTo(VisualElement bindable)
	{
		BindingContext = bindable.BindingContext;
		base.OnAttachedTo(bindable);
	}
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Aspects.Model;

internal class Class1
{
}

public readonly struct None : IEquatable<None>
{
	public static None Default { get; } = new();

	public bool Equals(None parameter) => parameter == Default;

	public override bool Equals(object? obj) => obj is None;

	public override int GetHashCode() => 0;

	public override string ToString() => "()";

	public static bool operator ==(None _, None __) => true;

	public static bool operator !=(None _, None __) => false;
}

public interface IActivityAware : ICommand<bool>;

public interface ICommand : ICommand<None>;

public interface ICommand<in T>
{
	void Execute(T parameter);
}

public interface ILastChanceExceptionHandler : IConditionAware<Exception>, IStopAware<Exception>;
public interface IConditionAware : IConditionAware<None>;

public interface IConditionAware<in T>
{
	ICondition<T> Condition { get; }
}

public interface IStopAware<T> : IOperation<Stop<T>>;

public interface IStopAware : IOperation<CancellationToken>;

public interface IOperation<in T> : ISelect<T, ValueTask>;

public interface ISelect<in TIn, out TOut>
{
	TOut Get(TIn parameter);
}

public readonly record struct Stop<T>(T Subject, CancellationToken Token)
{
	public static implicit operator T(Stop<T> instance) => instance.Subject;
	public static implicit operator CancellationToken(Stop<T> instance) => instance.Token;
}

public interface ICondition : ICondition<None>;
public interface ICondition<in T> : ISelect<T, bool>;
public interface IResult<out T>
{
	T Get();
}

public interface IValidationModel : ICondition
{
	bool IsValid { get; set; }
	Dictionary<string, string[]> Local { get; }
	Dictionary<string, string[]> External { get; }
}

public interface IValidationAware : ICommand<ValidationModelRecord>, IResult<IValidationModel>;
public readonly record struct ValidationModelRecord(Dictionary<string, string[]> Local, Dictionary<string, string[]> External);

public partial class ModelBase : ObservableObject, IActivityAware
{
	[ObservableProperty]
	public partial bool IsActive { get; set; }

	public void Execute(bool parameter)
	{
		IsActive = parameter;
	}
}

public abstract class ContentPage<T> : ContentPage
{
	protected ContentPage(T context) => BindingContext = context;
}

public abstract partial class ModelBase<T> : ModelBase where T : class
{
	[ObservableProperty]
	public virtual partial T? Subject { get; protected set; }

	[RelayCommand, ActivityAware, ExceptionAware]
	protected virtual async Task Initialize(CancellationToken parameter)
	{
		Subject = await ComposeSubject(parameter).ConfigureAwait(false);
	}

	protected abstract Task<T> ComposeSubject(CancellationToken parameter);
}

public abstract partial class UpdateModelBase<T> : ModelBase<T> where T : class
{
	readonly IMutable<T?> _last;

	protected UpdateModelBase() : this(new Copied<T>()) {}

	protected UpdateModelBase(IMutable<T?> last) => _last = last;

	protected override async Task Initialize(CancellationToken parameter)
	{
		await base.Initialize(parameter).ConfigureAwait(false);
		_last.Execute(Subject);
	}

	protected abstract Task<bool> UpdateSubject(Stop<T> parameter);

	protected virtual bool CanUpdate() => Subject is not null;

	[RelayCommand(CanExecute = nameof(CanUpdate)), ActivityAware, ExceptionAware]
	protected virtual async Task Update(CancellationToken parameter)
	{
		var view = _last.Get();
		if (view is not null && Subject != view)
		{
			var result = await UpdateSubject(new(Subject!, parameter)).ConfigureAwait(true);
			if (result)
			{
				await Initialize(parameter).ConfigureAwait(false);
			}
		}
	}
}
using Aspects;
using Aspects.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiApp7;

public class MainModelHierarchy : UpdateModelBase<Subject>
{
	protected override Task<Subject> ComposeSubject(CancellationToken parameter) => Task.FromResult(new Subject());

	protected override Task<bool> UpdateSubject(Stop<Subject> parameter) => Task.FromResult(true);
}

public sealed record Subject();

public partial class MainModel : ModelBase
{
	readonly IMutable<Subject?> _last;

	[ActivatorUtilitiesConstructor]
	public MainModel() : this(new Copied<Subject>()) {}

	public MainModel(IMutable<Subject?> last) => _last = last;

	[ObservableProperty]
	public virtual partial Subject? Subject { get; protected set; }

	[RelayCommand, ActivityAware, ExceptionAware]
	protected virtual async Task Initialize(CancellationToken parameter)
	{
		Subject = await Task.FromResult(new Subject()).ConfigureAwait(false);
		_last.Execute(Subject);
	}

	protected virtual bool CanUpdate() => Subject is not null;

	[RelayCommand(CanExecute = nameof(CanUpdate)), ActivityAware, ExceptionAware]
	protected virtual async Task Update(CancellationToken parameter)
	{
		var view = _last.Get();
		if (view is not null && Subject != view)
		{
			var result = await Task.FromResult(true).ConfigureAwait(true);
			if (result)
			{
				await Initialize(parameter).ConfigureAwait(false);
			}
		}
	}
}
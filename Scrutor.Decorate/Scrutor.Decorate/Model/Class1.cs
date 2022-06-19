using DragonSpark.Compose;
using DragonSpark.Diagnostics.Logging;
using DragonSpark.Model.Operations;
using DragonSpark.Runtime;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Scrutor.Decorate.Model;

public class CancelAwareProcess<T> : IOperation<T> where T : ExternalProcess
{
	readonly IOperation<T>          _previous;
	readonly UpdateProcessStatus<T> _status;
	readonly IOperation<T>          _other;

	protected CancelAwareProcess(IOperation<T> previous, UpdateProcessStatus<T> status)
		: this(previous, status, EmptyOperation<T>.Default) {}

	protected CancelAwareProcess(IOperation<T> previous, UpdateProcessStatus<T> status, IOperation<T> other)
	{
		_previous = previous;
		_status   = status;
		_other    = other;
	}

	public async ValueTask Get(T parameter)
	{
		try
		{
			await _previous.Await(parameter);
		}
		catch (CancelProcessException error)
		{
			await _other.Await(parameter);
			await _status.Await(new(parameter, ProcessStatus.Canceled, error.Reason));
		}
	}
}

public class UpdateProcessStatus<T> : IOperation<UpdateProcessStatusInput<T>> where T : ExternalProcess
{
	readonly ISaveProcess<T> _edit;
	readonly ITime           _time;

	protected UpdateProcessStatus(ISaveProcess<T> edit) : this(edit, Time.Default) {}

	protected UpdateProcessStatus(ISaveProcess<T> edit, ITime time)
	{
		_edit = edit;
		_time = time;
	}

	public async ValueTask Get(UpdateProcessStatusInput<T> parameter)
	{
		var (process, status, message) = parameter;
		process.Updates.Add(new()
		{
			Created = _time.Get(), Status = status, Message = message
		});
		await _edit.Await(process);
	}
}

public class StatusAwareProcess<T> : IOperation<T> where T : ExternalProcess
{
	readonly IOperation<T> _previous;
	readonly Log           _log;

	protected StatusAwareProcess(IOperation<T> previous, Log log)
	{
		_previous = previous;
		_log      = log;
	}

	public ValueTask Get(T parameter)
	{
		var latest = Enumerable.MaxBy(parameter.Updates, x => x.Created)?.Status ?? ProcessStatus.Queued;

		switch (latest)
		{
			case ProcessStatus.Error:
			case ProcessStatus.Queued:
				return _previous.Get(parameter);
		}

		_log.Execute(parameter.Id, latest);
		throw new InvalidOperationException("Invalid status detected.");
	}

	public sealed class Log : LogError<Guid, ProcessStatus>
	{
		public Log(ILogger<Log> logger)
			: base(logger, $"{A.Type<T>().Name} '{{Identity}}' has an unexpected status of '{{Status}}'") {}
	}
}

public class Process<T> : Operation<T> where T : ExternalProcess
{
	protected Process(Saving<T> saving, ProcessPlan<T> plan) : base(plan.Get(saving)) {}
}

public class ProcessPlan<T> : Select<Saving<T>, IOperation<T>> where T : ExternalProcess
{
	protected ProcessPlan(IFirstStep<T> first, params IStep<T>[] steps)
		: this(first, HandledComplete<T>.Default, steps) {}

	protected ProcessPlan(IFirstStep<T> first, IComplete<T> complete, params IStep<T>[] steps)
		: base(steps.Aggregate(A.Selection(first), (current, next) => current.Select(next)).Select(complete)) {}
}

public delegate ValueTask Operate<in T>(T parameter);

public delegate ValueTask Operate();

public delegate ConfiguredValueTaskAwaitable Await();

public class Saving<T> where T : ExternalProcess
{
	protected Saving(ISaveProcess<T> process, IOperation<T> save)
	{
		Process = process;
		Save    = save;
	}

	public ISaveProcess<T> Process { get; }

	public IOperation<T> Save { get; }
}

public interface ISaveProcess<in T> : IOperation<T> where T : ExternalProcess {}
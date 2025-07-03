using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Aspects.Model;

internal class Class2;

public class Result<T> : IResult<T>
{
	public static implicit operator T(Result<T> result) => result.Get();

	readonly Func<T> _source;

	public Result(IResult<T> result) : this(result.Get) {}

	public Result(Func<T> source) => _source = source;

	public T Get() => _source();
}

public sealed class Copied<T> : Result<T?>, IMutable<T?> where T : notnull
{
	readonly IMutable<T?>   _previous;
	readonly IAlteration<T> _copy;

	public Copied() : this(new Variable<T>(), DefaultCopy<T>.Default) {}

	public Copied(IMutable<T?> previous, IAlteration<T> copy) : base(previous)
	{
		_previous = previous;
		_copy     = copy;
	}

	public void Execute(T? parameter)
	{
		var instance = parameter is not null ? _copy.Get(parameter) : default;
		_previous.Execute(instance);
	}
}

public sealed class DefaultSerializer<T> : Serializer<T> where T : notnull
{
	public static DefaultSerializer<T> Default { get; } = new();

	DefaultSerializer() : base(JsonSerializerOptions.Default) {}
}

public class Select<TIn, TOut> : ISelect<TIn, TOut>
{
	readonly Func<TIn, TOut> _source;

	public Select(ISelect<TIn, TOut> select) : this(select.Get) {}

	public Select(Func<TIn, TOut> select) => _source = select;

	public TOut Get(TIn parameter) => _source(parameter);
}


public class Formatter<T> : Select<T, string>, IFormatter<T>
{
	protected Formatter(ISelect<T, string> @select) : base(@select) {}

	protected Formatter(Func<T, string> @select) : base(@select) {}
}
public class Serialize<T> : IFormatter<T>
{
	readonly JsonSerializerOptions _options;

	protected Serialize() : this(JsonSerializerOptions.Default) {}

	public Serialize(JsonSerializerOptions options) => _options = options;

	public string Get(T parameter) => JsonSerializer.Serialize(parameter, _options);
}

public class Parser<T> : Select<string, T>, IParser<T>
{
	public Parser(ISelect<string, T> @select) : base(@select) {}

	public Parser(Func<string, T> @select) : base(@select) {}
}
public sealed class DefaultDeserialize<T> : Deserialize<T>
{
	public static DefaultDeserialize<T> Default { get; } = new();

	DefaultDeserialize() {}

	public DefaultDeserialize(JsonSerializerOptions options) : base(options) {}
}

public class Deserialize<T> : IParser<T?>
{
	readonly JsonSerializerOptions _options;

	protected Deserialize() : this(JsonSerializerOptions.Default) {}

	protected Deserialize(JsonSerializerOptions options) => _options = options;

	public T? Get(string parameter)
	{
		var deserialize = JsonSerializer.Deserialize<T>(parameter, _options);
		return deserialize;
	}
}
public class Serializer<T> : Formatter<T>, ISerializer<T> where T : notnull
{
	protected Serializer(JsonSerializerOptions options)
		: this(new Serialize<T>(options),
		       new Parser<T>(new DefaultDeserialize<T>(options)!)) {}

	protected Serializer(IFormatter<T> format, IParser<T> parser) : base(format)
	{
		Parser = parser;
	}

	public IParser<T> Parser { get; }
}

public sealed class DefaultCopy<T> : Copy<T> where T : notnull
{
	public static DefaultCopy<T> Default { get; } = new();

	DefaultCopy() : base(DefaultSerializer<T>.Default) {}
}
public class Copy<T> : IAlteration<T> where T : notnull
{
	readonly ISerializer<T> _serializer;

	protected Copy(ISerializer<T> serializer) => _serializer = serializer;

	public T Get(T parameter) => _serializer.Parser.Get(_serializer.Get(parameter));
}

public interface IFormatter<in T> : ISelect<T, string>;

public interface ISerializer<T> : IFormatter<T> where T : notnull
{
	public IParser<T> Parser { get; }
}
public readonly record struct TargetInput<T>(T Target, string Content);
public interface ITarget<T> : ICommand<TargetInput<T>> where T : notnull;

public interface IParser<out T> : ISelect<string, T>;

public interface IMutable<T> : IResult<T>, ICommand<T>;

public interface IAlteration<T> : ISelect<T, T>;

public class Variable<T> : IMutable<T?>
{
	readonly T?[] _store = new T[1];

	public Variable(T? instance = default) => Execute(instance!);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Get()
	{
		// ATTRIBUTION: https://x.com/SergioPedri/status/1228752877604265985
		// https://github.com/CommunityToolkit/dotnet/blob/657c6971a8d42655c648336b781639ed96c2c49f/src/CommunityToolkit.HighPerformance/Extensions/ArrayExtensions.1D.cs#L52
		ref var reference = ref MemoryMarshal.GetArrayDataReference(_store);
		ref var result    = ref Unsafe.Add(ref reference, (nint)0);
		return result!;
	}

	public void Execute(T? parameter)
	{
		_store[0] = parameter;
	}
}

sealed class Always<T> : ICondition<T>
{
	public static Always<T> Default { get; } = new();

	Always() {}

	public bool Get(T parameter) => true;
}
public class ConditionAware<T> : IConditionAware<T>
{
	public ConditionAware(ICondition<T> condition) => Condition = condition;

	public ICondition<T> Condition { get; }
}

public class Operation<T> : Select<T, ValueTask>, IOperation<T>
{
	public Operation(ISelect<T, ValueTask> select) : this(select.Get) {}

	public Operation(Func<T, ValueTask> select) : base(select) {}
}

sealed class Exceptions : IExceptions
{
	readonly IExceptionLogger _select;

	public Exceptions(IExceptionLogger select) => _select = select;

	public async ValueTask Get(ExceptionInput parameter) => await _select.Get(parameter).ConfigureAwait(false);
}

public interface ISelecting<in TIn, TOut> : ISelect<TIn, ValueTask<TOut>>;

public interface IExceptionLogger : ISelecting<ExceptionInput, Exception>;
public interface ILogException : ISelecting<LogExceptionInput, Exception>;
public readonly record struct LogExceptionInput(ILogger Logger, Exception Exception);
sealed class ExceptionLogger : IExceptionLogger
{
	readonly ILoggerFactory _factory;
	readonly ILogException  _log;

	public ExceptionLogger(ILoggerFactory factory, ILogException log)
	{
		_factory = factory;
		_log     = log;
	}

	public ValueTask<Exception> Get(ExceptionInput parameter)
	{
		var (owner, exception) = parameter;
		var logger = _factory.CreateLogger(owner);
		var result = _log.Get(new(logger, exception));
		return result;
	}
}

sealed class LogException : ILogException
{
	public static LogException Default { get; } = new();

	LogException() {}

	public ValueTask<Exception> Get(LogExceptionInput parameter)
	{
		var (logger, exception) = parameter;
		logger.LogError(exception, "A problem was encountered while performing this operation");
		return ValueTask.FromResult(exception);
	}
}

public interface IExceptions : IOperation<ExceptionInput>;
public readonly record struct ExceptionInput(Type Owner, Exception Exception);

sealed class ApplicationErrorHandler : IApplicationErrorHandler
{
	readonly IExceptions _exceptions;
	readonly Type        _owner;

	[ActivatorUtilitiesConstructor]
	public ApplicationErrorHandler(IExceptions exceptions) : this(exceptions, typeof(ApplicationErrorHandler)) {}

	public ApplicationErrorHandler(IExceptions exceptions, Type owner)
	{
		_exceptions = exceptions;
		_owner      = owner;
	}

	public void Execute(Exception parameter)
	{
		_exceptions.Get(new(_owner, parameter)).GetAwaiter().GetResult();
	}
}

public interface IApplicationErrorHandler : ICommand<Exception>;
sealed class LastChanceExceptionHandler : ConditionAware<Exception>, ILastChanceExceptionHandler
{
	readonly IApplicationErrorHandler _handler;

	public LastChanceExceptionHandler(IApplicationErrorHandler handler) : this(handler, Always<Exception>.Default) {}

	public LastChanceExceptionHandler(IApplicationErrorHandler handler, ICondition<Exception> condition)
		: base(condition)
		=> _handler = handler;

	public ValueTask Get(Stop<Exception> parameter)
	{
		_handler.Execute(parameter);
		return ValueTask.CompletedTask;
	}
}
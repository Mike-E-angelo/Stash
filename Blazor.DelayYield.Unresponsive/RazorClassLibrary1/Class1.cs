using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace RazorClassLibrary1
{
	public class Resulting<T> : Result<Task<T>>, IResulting<T>
	{
		public Resulting(IResult<Task<T>> result) : base(result) {}

		public Resulting(Func<Task<T>> source) : base(source) {}
	}

	public class Result<T> : IResult<T>
	{
		public static implicit operator T(Result<T> result) => result.Get();

		readonly Func<T> _source;

		public Result(IResult<T> result) : this(result.Get) {}

		public Result(Func<T> source) => _source = source;

		public T Get() => _source();
	}


	public class Instance<T> : IResulting<T>
	{
		readonly T _instance;

		public Instance(T instance) => _instance = instance;

		public Task<T> Get() => Task.FromResult(_instance);
	}

	public sealed class Defaulting<T> : Instance<T?>
	{
		public static Defaulting<T> Default { get; } = new();

		Defaulting() : base(default) {}
	}

	public interface IResult<out T>
	{
		T Get();
	}

	public interface IResulting<T> : IResult<Task<T>> {}

	public class ActiveContentTemplateComponentBase<T> : ContentTemplateComponentBase<T>
	{
		[Parameter]
		public RenderFragment LoadingTemplate { get; set; } = x => x.AddContent(0, "Loading...");
	}

	public class ContentTemplateComponentBase<T> : ComponentBase
	{
		[Parameter]
		public RenderFragment<T> ChildContent { get; set; } = default!;

		[Parameter]
		public RenderFragment NotAssignedTemplate { get; set; } = x => x.AddContent(0, "Not Assigned");

		[Parameter]
		public RenderFragment ExceptionTemplate { get; set; } = x => x.AddContent(0, "A problem was encountered");
	}
}

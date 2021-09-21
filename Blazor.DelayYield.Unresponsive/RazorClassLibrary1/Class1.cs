using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace RazorClassLibrary1
{
	public sealed class HelloWorld
	{
		readonly Func<Task<string?>> _source;

		public HelloWorld(Func<Task<string?>> source) => _source = source;

		public Task<string?> Get() => _source();
	}

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

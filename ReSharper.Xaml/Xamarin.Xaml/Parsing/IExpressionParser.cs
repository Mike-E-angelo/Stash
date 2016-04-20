using System;

namespace Xamarin.Xaml
{
	internal interface IExpressionParser
	{
		object Parse(string match, ref string expression, IServiceProvider serviceProvider);
	}
	/*internal interface IExpressionParser<out T> : IExpressionParser where T : class
	{
		T Parse(string match, ref string expression, IServiceProvider serviceProvider);
	}*/

	static class ExpressionParserExtensions
	{
		public static T Parse<T>( this IExpressionParser @this, string match, ref string expression, IServiceProvider serviceProvider )
		{
			var result = (T)@this.Parse( match, ref expression, serviceProvider );
			return result;
		}
	}
}

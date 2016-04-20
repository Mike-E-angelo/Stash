using System;
using System.Text;

namespace Xamarin.Xaml
{
	public abstract class MarkupExpressionParser
	{
		public object ParseExpression(ref string expression, IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			if (expression.StartsWith("{}", StringComparison.Ordinal))
			{
				return expression.Substring(2);
			}
			if (expression[expression.Length - 1] != '}')
			{
				throw new Exception("Expression must end with '}'");
			}
			string match;
			int startIndex;
			if (!MarkupExpressionParser.MatchMarkup(out match, expression, out startIndex))
			{
				return false;
			}
			expression = expression.Substring(startIndex).TrimStart(new char[0]);
			if (expression.Length == 0)
			{
				throw new Exception("Expression did not end in '}'");
			}
			IExpressionParser expressionParser = Activator.CreateInstance(base.GetType()) as IExpressionParser;
			return expressionParser.Parse(match, ref expression, serviceProvider);
		}
		internal static bool MatchMarkup(out string match, string expression, out int end)
		{
			if (expression.Length < 2)
			{
				end = 1;
				match = null;
				return false;
			}
			if (expression[0] != '{')
			{
				end = 2;
				match = null;
				return false;
			}
			bool flag = false;
			int i;
			for (i = 1; i < expression.Length; i++)
			{
				if (expression[i] != ' ')
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				end = 3;
				match = null;
				return false;
			}
			int num = 0;
			while (num + i < expression.Length && expression[i + num] != ' ' && expression[i + num] != '}')
			{
				num++;
			}
			if (i + num == expression.Length)
			{
				end = 6;
				match = null;
				return false;
			}
			end = i + num;
			match = expression.Substring(i, num);
			return true;
		}
		protected void HandleProperty(string prop, IServiceProvider serviceProvider, ref string remaining, bool isImplicit)
		{
			object obj = null;
			if (isImplicit)
			{
				this.SetPropertyValue(null, prop, null, serviceProvider);
				return;
			}
			string strValue;
			if (remaining.StartsWith("{", StringComparison.Ordinal))
			{
				obj = this.ParseExpression(ref remaining, serviceProvider);
				remaining = remaining.TrimStart(new char[0]);
				if (remaining.Length > 0 && remaining[0] == ',')
				{
					remaining = remaining.Substring(1);
				}
				strValue = (obj as string);
			}
			else
			{
				char c;
				strValue = this.GetNextPiece(ref remaining, out c);
			}
			this.SetPropertyValue(prop, strValue, obj, serviceProvider);
		}
		protected abstract void SetPropertyValue(string prop, string strValue, object value, IServiceProvider serviceProvider);
		protected string GetNextPiece(ref string remaining, out char next)
		{
			bool flag = false;
			int num = 0;
			char c = '\0';
			remaining = remaining.TrimStart(new char[0]);
			if (remaining.Length == 0)
			{
				next = '￿';
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (num < remaining.Length && (flag || (remaining[num] != '}' && remaining[num] != ',' && remaining[num] != '=')))
			{
				if (flag)
				{
					if (remaining[num] == c)
					{
						flag = false;
						num++;
						break;
					}
				}
				else if (remaining[num] == '\'' || remaining[num] == '"')
				{
					flag = true;
					c = remaining[num];
					num++;
					continue;
				}
				if (remaining[num] == '\\')
				{
					num++;
					if (num == remaining.Length)
					{
						break;
					}
				}
				stringBuilder.Append(remaining[num]);
				num++;
			}
			if (flag && num == remaining.Length)
			{
				throw new Exception("Unterminated quoted string");
			}
			if (num == remaining.Length && !remaining.EndsWith("}", StringComparison.Ordinal))
			{
				throw new Exception("Binding did not end with '}'");
			}
			if (num == 0)
			{
				next = '￿';
				return null;
			}
			next = remaining[num];
			remaining = remaining.Substring(num + 1);
			while (stringBuilder.Length > 0 && char.IsWhiteSpace(stringBuilder[stringBuilder.Length - 1]))
			{
				stringBuilder.Length--;
			}
			if (stringBuilder.Length >= 2)
			{
				char c2 = stringBuilder[0];
				char c3 = stringBuilder[stringBuilder.Length - 1];
				if ((c2 == '\'' && c3 == '\'') || (c2 == '"' && c3 == '"'))
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					stringBuilder.Remove(0, 1);
				}
			}
			return stringBuilder.ToString();
		}
	}
}

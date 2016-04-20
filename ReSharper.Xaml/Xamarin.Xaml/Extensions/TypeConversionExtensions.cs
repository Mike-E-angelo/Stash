using System;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	internal static class TypeConversionExtensions
	{
		internal static object ConvertTo(this object value, Type toType, Func<ParameterInfo> pinfoRetriever, IServiceProvider serviceProvider)
		{
			/*Func<TypeConverter> getConverter = delegate
			{
				ParameterInfo element;
				if ( pinfoRetriever != null && ( element = pinfoRetriever() ) != null )
				{
					TypeConverterAttribute typeConverterAttribute = element.GetCustomAttribute<TypeConverterAttribute>();
					if ( typeConverterAttribute != null )
					{
						Type type = Type.GetType( typeConverterAttribute.ConverterTypeName );
						return (TypeConverter)Activator.CreateInstance( type );
					}
				}
				return null;
			};*/
			var result = value.ConvertTo(toType, pinfoRetriever != null ? new Func<TypeConverterAttribute>( () => pinfoRetriever().GetCustomAttribute<TypeConverterAttribute>( false ) ) : null , serviceProvider);
			return result;
		}
		internal static object ConvertTo(this object value, Type toType, Func<MemberInfo> minfoRetriever, IServiceProvider serviceProvider)
		{
			/*Func<TypeConverter> getConverter = delegate
			{
				MemberInfo element;
				if ( minfoRetriever != null && ( element = minfoRetriever() ) != null )
				{
					var attribute = element ?? toType.GetTypeInfo().GetCustomAttribute<TypeConverterAttribute>( false );
					if ( attribute != null )
					{
						var type = Type.GetType( attribute.ConverterTypeName );
						return (TypeConverter)Activator.CreateInstance( type );
					}
				}
				return null;
			};*/
			var result = value.ConvertTo(toType, minfoRetriever != null ? new Func<TypeConverterAttribute>( () => minfoRetriever().GetCustomAttribute<TypeConverterAttribute>( false ) ) : null , serviceProvider);
			return result;
		}

		internal static object ConvertTo(this object value, Type toType, Func<TypeConverterAttribute> attributeRetriever, IServiceProvider serviceProvider)
		{
			Func<TypeConverter> getConverter = delegate
			{
				if ( attributeRetriever != null )
				{
					var attribute = attributeRetriever() ?? toType.GetTypeInfo().GetCustomAttribute<TypeConverterAttribute>( false );
					if ( attribute != null )
					{
						var type = Type.GetType( attribute.ConverterTypeName );
						return (TypeConverter)Activator.CreateInstance( type );
					}
				}
				return null;
			};
			return value.ConvertTo(toType, getConverter, serviceProvider);
		}

		internal static object ConvertTo(this object value, Type toType, Func<TypeConverter> getConverter, IServiceProvider serviceProvider)
		{
			if ( value != null )
			{
				var conversion = serviceProvider.GetService<ITypeConversionService>().Convert( value, toType, getConverter != null ? getConverter() : null, serviceProvider );
				var result = conversion != null ? conversion.Value : Convert( value, toType ) ?? Implicit( value, toType );
				return result;
			}
			return null;
		}

		static object Convert( object value, Type toType )
		{
			string text = value as string;
			if ( text != null )
			{
				/*if (toType.GetTypeInfo().IsEnum)
				{
					return Enum.Parse(toType, text);
				}
				if (toType == typeof(int))
				{
					return int.Parse(text, CultureInfo.InvariantCulture);
				}
				if (toType == typeof(float))
				{
					return float.Parse(text, CultureInfo.InvariantCulture);
				}
				if (toType == typeof(double))
				{
					return double.Parse(text, CultureInfo.InvariantCulture);
				}
				if (toType == typeof(bool))
				{
					return bool.Parse(text);
				}
				if (toType == typeof(TimeSpan))
				{
					return TimeSpan.Parse(text, CultureInfo.InvariantCulture);
				}
				if (toType == typeof(DateTime))
				{
					return DateTime.Parse(text, CultureInfo.InvariantCulture);
				}
				if (toType == typeof(string) && text.StartsWith("{}", StringComparison.Ordinal))
				{
					return text.Substring(2);
				}
				if (toType == typeof(string))
				{
					return value;
				}*/
				var input = toType == typeof(string) && text.StartsWith( "{}", StringComparison.Ordinal ) ? text.Substring( 2 ) : null;
				var result = System.Convert.ChangeType( input, toType );
				return result;
			}
			return null;
		}

		static object Implicit( object value, Type toType )
		{
			MethodInfo runtimeMethod = value.GetType().GetRuntimeMethod( "op_Implicit", new[]
			{
				value.GetType()
			} );
			if ( runtimeMethod != null && runtimeMethod.ReturnType == toType )
			{
				value = runtimeMethod.Invoke( null, new[]
				{
					value
				} );
			}
			return value;
		}
	}
}

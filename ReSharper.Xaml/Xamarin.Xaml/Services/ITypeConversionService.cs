using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamarin.Xaml
{
	public interface ITypeConverter
	{
		bool CanConvertTo( Type from, Type to );

		object ConvertTo( object value, Type to, IServiceProvider serviceProvider );
	}

	public class TypeConversionResult
	{
		readonly object value;

		public TypeConversionResult( object value )
		{
			this.value = value;
		}

		public object Value
		{
			get { return value; }
		}
	}

	public interface ITypeConversionService
	{
		TypeConversionResult Convert( object value, Type to, TypeConverter converter, IServiceProvider serviceProvider );
	}

	public class TypeConversionService : ITypeConversionService
	{
		public static TypeConversionService Instance
		{
			get { return InstanceField; }
		}	static readonly TypeConversionService InstanceField = new TypeConversionService();

		readonly ITypeConverter external;

		public TypeConversionService() : this( null )
		{}

		public TypeConversionService( ITypeConverter external )
		{
			this.external = external;
		}

		public TypeConversionResult Convert( object value, Type to, TypeConverter converter, IServiceProvider serviceProvider )
		{
			if ( value != null )
			{
				var from = value.GetType();
				var result = FromExternal( value, from, to, serviceProvider )
							 ??
							 From( value, from, to, converter, serviceProvider );
				return result;
			}
			return null;
		}

		TypeConversionResult FromExternal( object value, Type from, Type to, IServiceProvider serviceProvider )
		{
			var result = external != null && external.CanConvertTo( from, to ) ? new TypeConversionResult( external.ConvertTo( value, to, serviceProvider ) ) : null;
			return result;
		}

		static TypeConversionResult From( object value, Type from, Type to, TypeConverter converter, IServiceProvider serviceProvider )
		{
			var result = converter != null && converter.CanConvertFrom( from ) ? new TypeConversionResult( converter.ConvertFrom( CultureInfo.InvariantCulture, value, serviceProvider ) ) : null;
			return result;
		}
	}
}
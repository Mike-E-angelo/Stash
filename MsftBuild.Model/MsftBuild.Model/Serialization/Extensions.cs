using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace MsftBuild.Model.Serialization
{
	public static class Extensions
	{
		public static T ToObject<T>(this IDictionary<string, object> source ) where T : class, new() => Into( source, new T() );

		/// <summary>
		/// Attribution: http://stackoverflow.com/a/4944547/3602057
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="current"></param>
		/// <returns></returns>
		public static T Into<T>( this IDictionary<string, object> source, T current )
		{
			var type = current.GetType();

			foreach ( var item in source )
			{
				type.GetProperty( item.Key ).SetValue( current, item.Value, null );
			}

			return current;
		}


		public static T Load<T>( this ISerializer @this, string data ) => @this.Load<T>( new MemoryStream( Encoding.Default.GetBytes( data ) ) );

		public static string GetTypeName( this Newtonsoft.Json.JsonSerializer @this, Type type ) => GetTypeName( type, @this.TypeNameAssemblyFormat, @this.Binder );

		/// <summary>
		/// Attribution: Newtonsoft.Json.Utilities.ReflectionUtils
		/// </summary>
		/// <param name="t"></param>
		/// <param name="assemblyFormat"></param>
		/// <param name="binder"></param>
		/// <returns></returns>
		static string GetTypeName( Type t, FormatterAssemblyStyle assemblyFormat, SerializationBinder binder )
		{
			string fullyQualifiedTypeName;
			if ( binder != null )
			{
				string assemblyName;
				string typeName;
				binder.BindToName( t, out assemblyName, out typeName );
				fullyQualifiedTypeName = typeName + ( assemblyName == null ? "" : ", " + assemblyName );
			}
			else
				fullyQualifiedTypeName = t.AssemblyQualifiedName;

			switch ( assemblyFormat )
			{
				case FormatterAssemblyStyle.Simple:
					return RemoveAssemblyDetails( fullyQualifiedTypeName );
				case FormatterAssemblyStyle.Full:
					return fullyQualifiedTypeName;
			}
			throw new ArgumentOutOfRangeException();
		}

		static string RemoveAssemblyDetails( string fullyQualifiedTypeName )
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag1 = false;
			bool flag2 = false;
			for ( int index = 0; index < fullyQualifiedTypeName.Length; ++index )
			{
				char ch = fullyQualifiedTypeName[index];
				switch ( ch )
				{
					case ',':
						if ( !flag1 )
						{
							flag1 = true;
							stringBuilder.Append( ch );
							break;
						}
						flag2 = true;
						break;
					case '[':
						flag1 = false;
						flag2 = false;
						stringBuilder.Append( ch );
						break;
					case ']':
						flag1 = false;
						flag2 = false;
						stringBuilder.Append( ch );
						break;
					default:
						if ( !flag2 )
						{
							stringBuilder.Append( ch );
						}
						break;
				}
			}
			return stringBuilder.ToString();
		}
	}
}
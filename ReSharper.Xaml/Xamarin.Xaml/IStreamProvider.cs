using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public interface IStreamProvider
	{
		Stream GetStream();
	}

	public class ResourceStreamProvider : IStreamProvider
	{
		static readonly Dictionary<Type, string> XamlResources = new Dictionary<Type, string>();

		readonly Type type;

		public ResourceStreamProvider( Type type )
		{
			this.type = type;
		}

		public Stream GetStream()
		{
			var contents = ToResourceString( type );
			var result = new MemoryStream( Encoding.UTF8.GetBytes( contents ) );
			return result;
		}

		static string ToResourceString( Type type )
		{
			string cached;
			var assembly = type.GetTypeInfo().Assembly;
			if ( XamlResources.TryGetValue( type, out cached ) )
			{
				return cached;
				/*var str2 = ReadResourceAsXaml( type, assembly, str );
				if ( str2 != null )
				{
					return str2;
				}*/
			}
			var filename = type.Name + ".xaml";
			var manifestResourceNames = assembly.GetManifestResourceNames();
			string result = null;
			foreach ( var str6 in manifestResourceNames.Where( str6 => ResourceMatchesFilename( assembly, str6, filename ) ) )
			{
				result = ReadResourceAsXaml( type, assembly, str6 );
				if ( result != null )
				{
					goto Label_0111;
				}
			}
			foreach ( var str7 in manifestResourceNames.Where( str7 => str7.EndsWith( ".xaml", StringComparison.OrdinalIgnoreCase ) ) )
			{
				result = ReadResourceAsXaml( type, assembly, str7 );
				if ( result != null )
				{
					goto Label_0111;
				}
			}
			foreach ( var str8 in manifestResourceNames.Where( str8 => !str8.EndsWith( ".xaml", StringComparison.OrdinalIgnoreCase ) ) )
			{
				result = ReadResourceAsXaml( type, assembly, str8, true );
				if ( result != null )
				{
					break;
				}
			}
			Label_0111:
			if ( string.IsNullOrEmpty( result ) )
			{
				throw new XamlParseException( string.Format( "No embeddedresources found for {0}", type ) );
			}
			XamlResources[type] = result;
			return result;
		}

		static bool ResourceMatchesFilename( Assembly assembly, string resource, string filename )
		{
			var manifestResourceInfo = assembly.GetManifestResourceInfo( resource );
			return ( !string.IsNullOrEmpty( manifestResourceInfo.FileName ) && string.Compare( manifestResourceInfo.FileName, filename, StringComparison.OrdinalIgnoreCase ) == 0 ) || ( resource.EndsWith( "." + filename, StringComparison.OrdinalIgnoreCase ) || string.Compare( resource, filename, StringComparison.OrdinalIgnoreCase ) == 0 );
		}

		static string ReadResourceAsXaml( Type type, Assembly assembly, string likelyTargetName, bool validate = false )
		{
			using ( var manifestResourceStream = assembly.GetManifestResourceStream( likelyTargetName ) )
			{
				using ( var streamReader = new StreamReader( manifestResourceStream ) )
				{
					if ( validate )
					{
						var c = (char)streamReader.Read();
						while ( char.IsWhiteSpace( c ) )
						{
							c = (char)streamReader.Read();
						}
						if ( c != '<' )
						{
							return null;
						}
						manifestResourceStream.Seek( 0L, SeekOrigin.Begin );
					}
					var text = streamReader.ReadToEnd();
					var pattern = string.Format( @"x:Class *= *""{0}""", type.FullName );
					var regex = new Regex( pattern, RegexOptions.ECMAScript );
					if ( regex.IsMatch( text ) || text.Contains( string.Format( @"x:Class=""{0}""", type.FullName ) ) )
					{
						return text;
					}
				}
			}
			return null;
		}
	}
}
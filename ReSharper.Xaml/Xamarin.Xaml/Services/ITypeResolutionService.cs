using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public interface ITypeResolutionService
	{
		Type GetElementType( XmlType xmlType, IXmlLineInfo xmlInfo, Assembly currentAssembly );
	}

	public class TypeResolutionService : ITypeResolutionService
	{
		public static TypeResolutionService Instance
		{
			get { return InstanceField; }
		}	static readonly TypeResolutionService InstanceField = new TypeResolutionService();

		public virtual Type GetElementType( XmlType xmlType, IXmlLineInfo xmlInfo, Assembly currentAssembly )
		{
			var locations = DetermineLocationCandidates( currentAssembly, xmlType.Name.NamespaceUri );
			var candidates = DetermineCandidates( xmlType );
			var type = locations.SelectMany( location => candidates.Select( candidate => location.Assembly.GetType( string.Concat( location.Namespace, ".", candidate ) ) ) ).FirstOrDefault( t => t != null );
			var result = type != null && xmlType.TypeArguments.Any()
				? type.MakeGenericType( xmlType.TypeArguments.Select( xmltype => GetElementType( xmltype, xmlInfo, currentAssembly ) ).ToArray() )
				: type;
			if ( result == null )
			{
				throw new XamlParseException( string.Format( "Type {0} not found in xmlns {1}", xmlType.Name.LocalName, xmlType.Name.NamespaceUri ), xmlInfo );
			}
			return result;
		}

		protected virtual string[] DetermineCandidates( XmlType xmlType )
		{
			var result = new[] { xmlType.Name.LocalName, xmlType.Name.LocalName + "Extension" }.Select( name =>
			{
				var scrubbed = name.Contains( ":" ) ? name.Substring( name.LastIndexOf( ':' ) + 1 ) : name;
				var item = xmlType.TypeArguments.Any() ? string.Concat( scrubbed, "`", xmlType.TypeArguments.Length ) : scrubbed;
				return item;
			} ).ToArray();

			return result;
		}

		protected struct TypeLocation
		{
			readonly Assembly assembly;
			readonly string @namespace;

			public TypeLocation( Type type ) : this( type.GetTypeInfo().Assembly, type.Namespace )
			{}

			public TypeLocation( Assembly assembly, string @namespace )
			{
				this.assembly = assembly;
				this.@namespace = @namespace;
			}

			public Assembly Assembly
			{
				get { return assembly; }
			}

			public string Namespace
			{
				get { return @namespace; }
			}

			public static implicit operator TypeLocation( Type type )
			{
				return new TypeLocation( type );
			}
			

			public bool Equals( TypeLocation other )
			{
				return Equals( assembly, other.assembly ) && string.Equals( @namespace, other.@namespace );
			}

			public override bool Equals( object obj )
			{
				if ( ReferenceEquals( null, obj ) )
				{
					return false;
				}
				return obj is TypeLocation && Equals( (TypeLocation)obj );
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ( ( assembly != null ? assembly.GetHashCode() : 0 ) * 397 ) ^ ( @namespace != null ? @namespace.GetHashCode() : 0 );
				}
			}
		}

		protected virtual TypeLocation[] DetermineLocationCandidates( Assembly currentAssembly, string namespaceUri )
		{
			if ( XmlnsHelper.IsCustom( namespaceUri ) )
			{
				switch ( namespaceUri )
				{
					case KnownSchemas.Xaml2006:
					case KnownSchemas.Xaml2009:
						return DetermineSystemLocations( currentAssembly, namespaceUri );
				}
				return DetermineCustomLocations( currentAssembly, namespaceUri );
			}
			return new TypeLocation[] { typeof(View), typeof(XamlServices) };
		}

		protected virtual TypeLocation[] DetermineCustomLocations( Assembly currentAssembly, string namespaceUri )
		{
			string name, ns, assemblyName;
			XmlnsHelper.ParseXmlns( namespaceUri, out name, out ns, out assemblyName );
			return new[] { new TypeLocation( assemblyName == null ? currentAssembly : Assembly.Load( new AssemblyName( assemblyName ) ), ns ) };
		}

		protected virtual TypeLocation[] DetermineSystemLocations( Assembly currentAssembly, string namespaceUri )
		{
			return new TypeLocation[] { typeof(XamlServices), typeof(object) };
		}
	}
}
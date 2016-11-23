using System.Collections.Generic;
using System.IO;

namespace MsftBuild.Model.Serialization
{
	public class SerializerLocator : ISerializerLocator
	{
		readonly static IDictionary<string, ISerializer> Registry =
			new Dictionary<string, ISerializer>
			{
				{ ".xaml", XamlSerializer.Default },
				{ ".xml", XmlSerializer.Default },
				{ ".json", JsonSerializer.Default }
			};

		public static SerializerLocator Default { get; } = new SerializerLocator();
		SerializerLocator() : this( Registry ) {}

		readonly IDictionary<string, ISerializer> registry;

		public SerializerLocator( IDictionary<string, ISerializer> registry )
		{
			this.registry = registry;
		}

		public ISerializer Locate( string fileName )
		{
			// Very rudimentary implementation here.  Simply do a lookup on file extension.
			// An ideal implementation would have a plugin registry of some sort that would
			// determine the correct serializer with more sophisticated strategies:
			ISerializer serializer;
			var extension = Path.GetExtension( fileName );
			var result = extension != null && registry.TryGetValue( extension, out serializer ) ? serializer : null;
			return result;
		}
	}
}
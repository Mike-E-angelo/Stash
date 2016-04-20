using System;
using System.IO;
using System.Xml;

namespace Xamarin.Xaml
{
	public static class XamlServices
	{
		public static T BuildUpFromResources<T>( this T @this, Type callingType = null )
		{
			var result = @this.BuildUpFrom( new ResourceStreamProvider( callingType ?? @this.GetType() ) );
			return result;
		}

		public static T Load<T>( Stream stream ) where T : new()
		{
			var result = new T().BuildUpFrom( stream );
			return result;
		}

		public static T Load<T>( IStreamProvider provider ) where T : new()
		{
			var result = new T().BuildUpFrom( provider );
			return result;
		}

		public static T BuildUpFrom<T>( this T @this, IStreamProvider provider )
		{
			using ( var stream = provider.GetStream() )
			{
				return @this.BuildUpFrom( stream );
			}
		}

		public static T BuildUpFrom<T>( this T @this, Stream stream )
		{
			using ( var reader = XmlReader.Create( stream ) )
			{
				while ( reader.Read() )
				{
					switch ( reader.NodeType )
					{
						case XmlNodeType.Element:
							var element = new ParsingEngine().Run( reader, @this );
							var context = new HydratationContext { RootElement = element };
							Visitor.Apply( context, element );
							break;
					}
				}
			}
			return @this;
		}
	}
}

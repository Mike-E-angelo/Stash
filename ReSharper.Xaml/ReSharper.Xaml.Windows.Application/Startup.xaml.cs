using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Converters;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Converters;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;
using Xamarin.Xaml;
using MemberDescriptor = Xamarin.Xaml.MemberDescriptor;

namespace ReSharper.Xaml.Windows.Application
{
	/// <summary>
	/// Interaction logic for Startup.xaml
	/// </summary>
	public partial class Startup
	{
		public Startup()
		{
			Services.Configure( registry =>
			{
				registry.Add<IVisitorProviderService>( VisitorProviderService.Instance );
				registry.Add<IObjectBuilderService>( ObjectBuilderService.Instance );
				registry.Add<ISupportInitializeService>( SupportInitializeService.Instance );
				registry.Add<ISupportPropertyInitializeService>( SupportPropertyInitializeService.Instance );
				registry.Add<ITypeResolutionService>( TypeResolutionService.Instance );
				registry.Add<ITypeConversionService>( new TypeConversionService( ComponentTypeConverter.Instance ) );
				registry.Add<IContentPropertyService>( ContentPropertyService.Instance );
				registry.Add<IPropertyValueSetterService>( PropertyValueSetterService.Instance );
			} );
		}
	}

	public class VisitorProviderService : Xamarin.Xaml.VisitorProviderService
	{
		public new static VisitorProviderService Instance
		{
			get { return InstanceField; }
		}	static readonly VisitorProviderService InstanceField = new VisitorProviderService();

		protected override IEnumerable<IXamlNodeVisitor> CreateVisitors( HydratationContext context, INode node )
		{
			return base.CreateVisitors( context, node );
		}
	}

	public class ObjectBuilderService : Xamarin.Xaml.ObjectBuilderService
	{
		public new static ObjectBuilderService Instance
		{
			get { return InstanceField; }
		}	static readonly ObjectBuilderService InstanceField = new ObjectBuilderService();

		public override object Create( IObjectCreationContext context )
		{
			return base.Create( context );
		}
	}

	public class SupportInitializeService : Xamarin.Xaml.SupportInitializeService
	{
		public new static SupportInitializeService Instance
		{
			get { return InstanceField; }
		}	static readonly SupportInitializeService InstanceField = new SupportInitializeService();

		public override void BeginInitialize( object item )
		{
			var support = item as ISupportInitialize;
			if ( support != null )
			{
				support.BeginInit();
			}
		}

		public override void EndInitialize( object item )
		{
			var support = item as ISupportInitialize;
			if ( support != null )
			{
				support.EndInit();
			}
		}
	}

	public class SupportPropertyInitializeService : Xamarin.Xaml.SupportPropertyInitializeService
	{
		public new static SupportPropertyInitializeService Instance
		{
			get { return InstanceField; }
		}	static readonly SupportPropertyInitializeService InstanceField = new SupportPropertyInitializeService();

		public override object BeginSetProperty( object item, MemberDescriptor descriptor, object value )
		{
			return base.BeginSetProperty( item, descriptor, value );
		}

		public override void EndSetProperty( object item, MemberDescriptor descriptor, object value )
		{
			base.EndSetProperty( item, descriptor, value );
		}
	}

	public class PropertyValueSetterService : Xamarin.Xaml.PropertyValueSetterService
	{
		public new static PropertyValueSetterService Instance
		{
			get { return InstanceField; }
		}	static readonly PropertyValueSetterService InstanceField = new PropertyValueSetterService();

		protected override bool? SetValueUsingReflection( PropertyValueContext context, PropertyInfo propertyInfo )
		{
			var target = context.Instance as DependencyObject;
			var binding = context.Value as Binding;
			if ( target != null && binding != null )
			{
				var property = DependencyPropertyDescriptor.FromName( context.Descriptor.MemberName, target.GetType(), target.GetType(), false );
				if ( property != null )
				{
					BindingOperations.SetBinding( target, property.DependencyProperty, binding );
					return true;
				}
			}
			return base.SetValueUsingReflection( context, propertyInfo );
		}
	}

	public class TypeResolutionService : Xamarin.Xaml.TypeResolutionService
	{
		const string Native = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		public new static TypeResolutionService Instance
		{
			get { return InstanceField; }
		}	static readonly TypeResolutionService InstanceField = new TypeResolutionService();

		protected override TypeLocation[] DetermineCustomLocations( Assembly currentAssembly, string namespaceUri )
		{
			var system = namespaceUri == Native ? new TypeLocation[]
			{
				// WindowsBase:
				typeof(Freezable),
				typeof(DataSourceProvider),
				typeof(RectValueSerializer),
				typeof(Dispatcher),

				// PresentationCore:
				typeof(Clipboard),
				typeof(DocumentPage),
				typeof(GestureRecognizer),
				typeof(Cursor),
				typeof(Brush),
				typeof(Animatable),
				typeof(GeometryValueSerializer),
				typeof(Effect),
				typeof(BitmapFrame),
				typeof(Camera),
				typeof(QuaternionConverter),
				typeof(TextBounds),
				typeof(BaseUriHelper),

				// PresentationFramework:
				typeof(Binding),
				typeof(DesignerProperties),
				typeof(System.Windows.Application),
				typeof(Annotation),
				typeof(AnnotationStore),
				typeof(Button),
				typeof(ButtonBase),
				typeof(Adorner),
				typeof(BlockElement),
				typeof(CommandConverter),
				typeof(TextOptions),
				typeof(BeginStoryboard),
				typeof(JournalEntry),
				typeof(ContentTypes),
				typeof(Ellipse),
				typeof(JumpItem)
			} : new TypeLocation[0];
			return base.DetermineCustomLocations( currentAssembly, namespaceUri ).Concat( system ).ToArray();
		}
	}

	public class ContentPropertyService : Xamarin.Xaml.ContentPropertyService
	{
		public new static ContentPropertyService Instance
		{
			get { return InstanceField; }
		}	static readonly ContentPropertyService InstanceField = new ContentPropertyService();

		readonly Lazy<Tuple<Type, string>[]> mappings;

		public ContentPropertyService()
		{
			mappings = new Lazy<Tuple<Type, string>[]>( DetermineMappings );
		}

		protected virtual Tuple<Type, string>[] DetermineMappings()
		{
			var result = new[]
			{
				new Tuple<Type, string>( typeof(Binding), "Path" ), 
				new Tuple<Type, string>( typeof(StaticResourceExtension), "ResourceKey" ), 
			};
			return result;
		}

		public override string GetContentProperty( Type target, IServiceProvider serviceProvider )
		{
			return base.GetContentProperty( target, serviceProvider ) ?? FromNative( target );
		}

		protected virtual string FromNative( Type target )
		{
			var attribute = target.GetCustomAttribute<ContentPropertyAttribute>();
			var result = attribute != null ? attribute.Name : mappings.Value.Where( tuple => tuple.Item1.IsAssignableFrom( target ) ).Select( tuple => tuple.Item2 ).FirstOrDefault();
			return result;
		}
	}

	public class ComponentTypeConverter : ITypeConverter
	{
		public static ComponentTypeConverter Instance
		{
			get { return InstanceField; }
		}	static readonly ComponentTypeConverter InstanceField = new ComponentTypeConverter();

		/*public override bool CanConvertFrom( Type sourceType )
		{
			return false;
			var result = TypeDescriptor.GetConverter( sourceType ).CanConvertFrom( sourceType );
			return result;
		}

		public override object ConvertFrom( CultureInfo culture, object value )
		{
			var result = TypeDescriptor.GetConverter( value.GetType() ).ConvertFrom( value );
			return result;
		}*/

		public bool CanConvertTo( Type from, Type to )
		{
			var fromConverter = TypeDescriptor.GetConverter( @from );
			var toConverter = TypeDescriptor.GetConverter( to );
			var result = fromConverter.CanConvertTo( to ) || toConverter.CanConvertFrom( @from );
			return result;
		}

		public object ConvertTo( object value, Type to, IServiceProvider serviceProvider )
		{
			var from = value.GetType();
			var fromConverter = TypeDescriptor.GetConverter( from );
			var toConverter = TypeDescriptor.GetConverter( to );
			var result = fromConverter.CanConvertTo( to ) ? fromConverter.ConvertTo( value, to )
				:
				toConverter.ConvertFrom( value );
			return result;
		}
	}
}

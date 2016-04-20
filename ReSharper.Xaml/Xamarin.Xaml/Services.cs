using System;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public delegate IServiceProvider CreateServiceProvider( HydratationContext context = null, INode node = null );
	public delegate void ConfigureRegistry( IServiceRegistry registry );

	public static class Services
	{
		static readonly ConfigureRegistry DefaultConfiguration = registry =>
		{
			registry.Add<IVisitorProviderService>( VisitorProviderService.Instance );
			registry.Add<IObjectBuilderService>( ObjectBuilderService.Instance );
			registry.Add<ISupportInitializeService>( SupportInitializeService.Instance );
			registry.Add<ISupportPropertyInitializeService>( SupportPropertyInitializeService.Instance );
			registry.Add<ITypeResolutionService>( TypeResolutionService.Instance );
			registry.Add<ITypeConversionService>( TypeConversionService.Instance );
			registry.Add<IContentPropertyService>( ContentPropertyService.Instance );
			registry.Add<IPropertyValueSetterService>( PropertyValueSetterService.Instance );
		};

		static CreateServiceProvider createProvider = ( context, node ) => new XamlServiceProvider( context, node );

		public static void SetCreator( CreateServiceProvider provider )
		{
			createProvider = provider;
		}
		
		public static void Configure( ConfigureRegistry configure )
		{
			current = configure;
		}	static ConfigureRegistry current;

		internal static IServiceProvider Default
		{
			get { return DefaultBuilder.Value; }
		}	readonly static Lazy<IServiceProvider> DefaultBuilder = new Lazy<IServiceProvider>( () => Create() );

		public static object CheckValue( this IServiceProvider provider, object value )
		{
			var markupExtension = value as IMarkupExtension;
			var valueProvider = value as IValueProvider;
			var result = markupExtension != null ? CheckValue( provider, markupExtension.ProvideValue( provider ) )
				: 
				valueProvider != null ?  CheckValue( provider, valueProvider.ProvideValue( provider ) ) : value;
			return result;
		}

		public static IServiceProvider Create( HydratationContext context = null, INode node = null, ConfigureRegistry configure = null )
		{
			var result = createProvider( context, node );
			var registry = result as IServiceRegistry;
			if ( registry != null )
			{
				 registry.Configured( configure );
			}
			return result;
		}

		public static T Configured<T>( this T @this, ConfigureRegistry configure = null ) where T : IServiceRegistry
		{
			foreach ( var registry in new[] { DefaultConfiguration, current, configure }.Where( registry => registry != null ) )
			{
				registry( @this );
			}
			return @this;
		}
	}
}
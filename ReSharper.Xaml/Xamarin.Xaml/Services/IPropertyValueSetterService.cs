using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public interface IPropertyValueSetterService
	{
		void SetPropertyValue( PropertyValueContext context );
	}

	public class PropertyValueSetterService : IPropertyValueSetterService
	{
		public static PropertyValueSetterService Instance
		{
			get { return InstanceField; }
		}	static readonly PropertyValueSetterService InstanceField = new PropertyValueSetterService();

		public void SetPropertyValue( PropertyValueContext context )
		{
			AddEventHandler( context );

			if ( !HandleBindableProperty( context ) )
			{
				var propertyInfo = context.Descriptor.DeclaringType.GetRuntimeProperties().FirstOrDefault( p => p.Name == context.Descriptor.MemberName );
				var assign = SetValueUsingReflection( context, propertyInfo );
				var assigned = assign.HasValue && assign.Value;
				if ( !assigned && !AddChild( context, propertyInfo ) )
				{
					var message = !assign.HasValue ? string.Format( "No Property of name {0} found", context.Descriptor.MemberName ) : string.Format( @"Cannot assign property ""{0}"": type mismatch between ""{1}"" and ""{2}""", context.Descriptor.MemberName, context.Value.GetType(), propertyInfo.PropertyType );
					throw new XamlParseException( message, context.LineInfo );
				}
			}
		}

		protected virtual bool HandleBindableProperty( PropertyValueContext context )
		{
			var bindableProperty = GetBindableProperty( context.Descriptor.DeclaringType, context.Descriptor.MemberName, context.LineInfo );
			var result = bindableProperty != null;
			if ( result && !ApplyDynamicResource( context, bindableProperty ) )
			{
				if ( context.Value is BindingBase )
				{
					ApplyBinding( context, bindableProperty );
				}
				else
				{
					return SetBindablePropertyValue( context, bindableProperty );
				}
			}
			return result;
		}

		protected virtual void AddEventHandler( PropertyValueContext context )
		{
			var runtimeEvent = context.Descriptor.DeclaringType.GetRuntimeEvent( context.Descriptor.MemberName );
			if ( runtimeEvent != null && context.Value is string )
			{
				var methodInfo = context.RootElement.GetType().GetRuntimeMethods().FirstOrDefault( mi => mi.Name == (string)context.Value );
				if ( methodInfo == null )
				{
					throw new XamlParseException( String.Format( "No method {0} found on type {1}", context.Value, context.RootElement.GetType() ), context.LineInfo );
				}
				try
				{
					runtimeEvent.AddEventHandler( context.Instance, methodInfo.CreateDelegate( runtimeEvent.EventHandlerType, context.RootElement ) );
				}
				catch ( ArgumentException )
				{
					throw new XamlParseException( String.Format( "Method {0} does not have the correct signature", context.Value ), context.LineInfo );
				}
			}
		}

		protected virtual bool ApplyDynamicResource( PropertyValueContext context, BindableProperty bindableProperty )
		{
			var resource = context.Value as DynamicResource;
			if ( resource != null )
			{
				if ( !context.Instance.GetType().GetTypeInfo().IsSubclassOf( typeof(BindableObject) ) )
				{
					throw new XamlParseException( String.Format( "{0} is not a BindableObject", context.Instance.GetType().Name ), context.LineInfo );
				}
				( (BindableObject)context.Instance ).SetDynamicResource( bindableProperty, resource.Key );
				return true;
			}
			return false;
		}

		protected virtual bool? SetValueUsingReflection( PropertyValueContext context, PropertyInfo propertyInfo )
		{
			MethodInfo setMethod;
			if ( propertyInfo != null && propertyInfo.CanWrite && ( setMethod = propertyInfo.SetMethod ) != null )
			{
				var value = context.Value.ConvertTo( propertyInfo.PropertyType, () => propertyInfo, context.ServiceProvider );
				if ( value == null || propertyInfo.PropertyType.IsInstanceOfType( value ) )
				{
					try
					{
						setMethod.Invoke( context.Instance, new[] { value } );
						return true;
					}
					catch ( ArgumentException )
					{
						return false;
					}
				}
			}
			return null;
		}

		protected virtual bool SetBindablePropertyValue( PropertyValueContext context, BindableProperty bindableProperty )
		{
			if ( !context.Instance.GetType().GetTypeInfo().IsSubclassOf( typeof(BindableObject) ) )
			{
				throw new XamlParseException( String.Format( "{0} is not a BindableObject", context.Instance.GetType().Name ), context.LineInfo );
			}
			var minfoRetriever = context.Descriptor != MemberDescriptor.None 
				? (Func<MemberInfo>)( () => context.Descriptor.DeclaringType.GetRuntimeMethod( "Get" + context.Descriptor.MemberName, new[] { typeof(BindableObject) } ) ) 
				: ( () => context.Descriptor.DeclaringType.GetRuntimeProperty( context.Descriptor.MemberName ) );

			var value = context.Value.ConvertTo( bindableProperty.ReturnType, minfoRetriever, context.ServiceProvider );
			if ( ( value == null && !bindableProperty.ReturnTypeInfo.IsValueType ) || bindableProperty.ReturnType.IsInstanceOfType( value ) )
			{
				( (BindableObject)context.Instance ).SetValue( bindableProperty, value );
				return true;
			}
			return false;
		}

		protected virtual bool AddChild( PropertyValueContext context, PropertyInfo propertyInfo )
		{
			MethodInfo getMethod;
			IEnumerable enumerable;
			if ( propertyInfo != null && propertyInfo.CanRead && ( getMethod = propertyInfo.GetMethod ) != null && ( enumerable = ( getMethod.Invoke( context.Instance, new object[0] ) as IEnumerable ) ) != null )
			{
				MethodInfo methodInfo2;
				if ( ( methodInfo2 = enumerable.GetType().GetRuntimeMethods().First( mi => mi.Name == "Add" && mi.GetParameters().Length == 1 ) ) != null )
				{
					methodInfo2.Invoke( enumerable, new[]
					{
						context.Value.ConvertTo( propertyInfo.PropertyType, (Func<TypeConverter>)null, context.ServiceProvider )
					} );
					return true;
				}
			}
			return false;
		}

		protected virtual void ApplyBinding( PropertyValueContext context, BindableProperty bindableProperty )
		{
			if ( !context.Instance.GetType().GetTypeInfo().IsSubclassOf( typeof(BindableObject) ) )
			{
				throw new XamlParseException( String.Format( "{0} is not a BindableObject", context.Instance.GetType().Name ), context.LineInfo );
			}
			( (BindableObject)context.Instance ).SetBinding( bindableProperty, context.Value as BindingBase );
		}

		static BindableProperty GetBindableProperty( Type elementType, string localName, IXmlLineInfo lineInfo )
		{
			var fieldInfo = elementType.GetFields().FirstOrDefault( fi => fi.Name == localName + "Property" && fi.IsStatic && fi.IsPublic );
			Exception ex = fieldInfo == null ? new XamlParseException( String.Format( "BindableProperty {0} not found on {1}", localName + "Property", elementType.Name ), lineInfo ) : null;
			var result = ex == null ? fieldInfo.GetValue( null ) as BindableProperty : null;
			return result;
		}
	}

	public struct MemberDescriptor
	{
		public readonly static MemberDescriptor None = new MemberDescriptor();

		readonly Type declaringType;
		readonly string memberName;

		public MemberDescriptor( Type declaringType, string memberName )
		{
			this.declaringType = declaringType;
			this.memberName = memberName;
		}

		public Type DeclaringType
		{
			get { return declaringType; }
		}

		public string MemberName
		{
			get { return memberName; }
		}

		public bool Equals( MemberDescriptor other )
		{
			return declaringType == other.declaringType && string.Equals( memberName, other.memberName );
		}

		public static bool operator ==(MemberDescriptor x1, MemberDescriptor x2)
		{
			return x1.Equals( x2 );
		}
		public static bool operator !=(MemberDescriptor x1, MemberDescriptor x2)
		{
			return !(x1 == x2);
		}

		public override bool Equals( object obj )
		{
			if ( ReferenceEquals( null, obj ) )
			{
				return false;
			}
			return obj is MemberDescriptor && Equals( (MemberDescriptor)obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ( ( declaringType != null ? declaringType.GetHashCode() : 0 ) * 397 ) ^ ( memberName != null ? memberName.GetHashCode() : 0 );
			}
		}
	}

	public class PropertyValueContext
	{
		readonly object instance;
		readonly object value;
		readonly object rootElement;
		readonly IXmlLineInfo lineInfo;
		readonly MemberDescriptor descriptor;
		readonly IServiceProvider serviceProvider;

		public PropertyValueContext( object instance, object value, object rootElement, IXmlLineInfo lineInfo, MemberDescriptor descriptor, IServiceProvider serviceProvider )
		{
			this.instance = instance;
			this.value = value;
			this.rootElement = rootElement;
			this.lineInfo = lineInfo;
			this.descriptor = descriptor;
			this.serviceProvider = serviceProvider;
		}

		public object Instance
		{
			get { return instance; }
		}

		public object Value
		{
			get { return value; }
		}

		public object RootElement
		{
			get { return rootElement; }
		}

		public IXmlLineInfo LineInfo
		{
			get { return lineInfo; }
		}

		public MemberDescriptor Descriptor
		{
			get { return descriptor; }
		}

		public IServiceProvider ServiceProvider
		{
			get { return serviceProvider; }
		}
	}
}
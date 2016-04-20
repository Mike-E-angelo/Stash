using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	public interface IObjectBuilderService
	{
		object Create( IObjectCreationContext context );
	}

	public class ObjectBuilderContext : IObjectCreationContext
	{
		readonly HydratationContext context;
		readonly Type type;
		readonly IElementNode node;
		readonly IServiceProvider serviceProvider;
		
		internal ObjectBuilderContext( HydratationContext context, Type type, IElementNode node, IServiceProvider serviceProvider )
		{
			this.context = context;
			this.type = type;
			this.node = node;
			this.serviceProvider = serviceProvider;
		}

		public Type Type
		{
			get { return type; }
		}

		public INode Source
		{
			get { return node; }
		}

		public IServiceProvider Provider
		{
			get { return serviceProvider; }
		}

		public object GetValue( object key )
		{
			var result = context.Values[(INode)key];
			return result;
		}

		public IServiceProvider CreateProvider( object key )
		{
			var result = Services.Create( context, (INode)key );
			return result;
		}
	}

	public interface IObjectCreationContext
	{
		Type Type { get; }

		INode Source { get; }

		IServiceProvider Provider { get; }

		object GetValue( object key );

		IServiceProvider CreateProvider( object key );
	}

	public class ObjectBuilderService : IObjectBuilderService
	{
		public static ObjectBuilderService Instance
		{
			get { return InstanceField; }
		}	static readonly ObjectBuilderService InstanceField = new ObjectBuilderService();

		public virtual object Create( IObjectCreationContext context )
		{
			var node = (IElementNode)context.Source;
			switch ( node.XmlType.Name.NamespaceUri )
			{
				case KnownSchemas.Xaml2009:
					return CreateLanguagePrimitive( context.Type, node );
			}
			
			if ( node.Properties.ContainsKey( XmlName.xArguments ) || node.Properties.ContainsKey( XmlName.xFactoryMethod ) )
			{
				return CreateFromFactory( context );
			}
			if ( context.Type.GetTypeInfo().DeclaredConstructors.Any( ci =>
			{
				if ( ci.IsPublic && ci.GetParameters().Length != 0 )
				{
					return ci.GetParameters().All( pi => pi.CustomAttributes.Any( attr => attr.AttributeType == typeof(ParameterAttribute) ) );
				}
				return false;
			} ) )
			{
				return CreateFromParameterizedConstructor( context );
			}
			try
			{
				if ( context.Type == typeof(DataTemplate) )
				{
					return new DataTemplate();
				}
				if ( node.CollectionItems.Any() && node.CollectionItems.First() is ValueNode )
				{
					var item = ( (ValueNode)node.CollectionItems.First() ).Value.ConvertTo( context.Type, () => context.Type.GetTypeInfo(), context.Provider );
					if ( item != null && context.Type.IsInstanceOfType( item ) )
					{
						return item;
					}
				}
				return Activator.CreateInstance( context.Type );
			}
			catch ( TargetInvocationException ex )
			{
				if ( ex.InnerException is XamlParseException || ex.InnerException is XmlException )
				{
					throw ex.InnerException;
				}
				throw;
			}
		}

		protected virtual object CreateFromParameterizedConstructor(IObjectCreationContext context)
		{
			var constructorInfo = context.Type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(delegate(ConstructorInfo ci)
			{
				return ci.IsPublic && ci.GetParameters().All( pi => pi.CustomAttributes.Any( attr => attr.AttributeType == typeof(ParameterAttribute)));
			} );
			var parameters = this.CreateArgumentsArray(context, constructorInfo);
			return constructorInfo.Invoke(parameters);
		}

		protected virtual object[] CreateArgumentsArray( IObjectCreationContext context )
		{
			var enode = (IElementNode)context.Source;
			if ( enode.Properties.ContainsKey( XmlName.xArguments ) )
			{
				var node = enode.Properties[XmlName.xArguments];
				var elementNode = node as ElementNode;
				if ( elementNode != null )
				{
					return new[] { context.GetValue( elementNode ) };
				}
				var listNode = node as ListNode;
				if ( listNode != null )
				{
					return listNode.CollectionItems.Select( context.GetValue ).ToArray();
				}
			}
			return null;
		}

		protected virtual object CreateFromFactory(IObjectCreationContext context)
		{
			var node = (IElementNode)context.Source;
			object[] array = this.CreateArgumentsArray(context);
			if (!node.Properties.ContainsKey(XmlName.xFactoryMethod))
			{
				return Activator.CreateInstance(context.Type, array);
			}
			string text = (string)((ValueNode)node.Properties[XmlName.xFactoryMethod]).Value;
			Type[] arg_75_0;
			if (array != null)
			{
				arg_75_0 = (
					from a in array
					select a.GetType()).ToArray<Type>();
			}
			else
			{
				arg_75_0 = new Type[0];
			}
			Type[] array2 = arg_75_0;
			MethodInfo runtimeMethod = context.Type.GetRuntimeMethod(text, array2);
			if (runtimeMethod == null || !runtimeMethod.IsStatic)
			{
				string arg_D9_0 = "No static method found for {0}::{1} ({2})";
				object[] array3 = new object[3];
				array3[0] = context.Type.FullName;
				array3[1] = text;
				array3[2] = string.Join(", ", 
					from t in array2
					select t.FullName);
				throw new MissingMemberException(string.Format(arg_D9_0, array3));
			}
			return runtimeMethod.Invoke(null, array);
		}

		protected virtual object[] CreateArgumentsArray(IObjectCreationContext context, ConstructorInfo ctorInfo)
		{
			var node = (IElementNode)context.Source;
			int num = ctorInfo.GetParameters().Length;
			object[] array = new object[num];
			for (int i = 0; i < num; i++)
			{
				ParameterInfo parameter = ctorInfo.GetParameters()[i];
				string text = parameter.CustomAttributes.First( attr => attr.AttributeType == typeof(ParameterAttribute)).ConstructorArguments.First<CustomAttributeTypedArgument>().Value as string;
				XmlName key = new XmlName("", text);
				INode key2;
				if (!node.Properties.TryGetValue(key, out key2))
				{
					throw new XamlParseException(string.Format("The Property {0} is required to create a {1} object.", new object[]
					{
						text,
						ctorInfo.DeclaringType.FullName
					}), node as IXmlLineInfo);
				}
				node.Properties.Remove(key);
				array[i] =  context.GetValue(key2).ConvertTo(parameter.ParameterType, () => parameter, context.CreateProvider(key2));
			}
			return array;
		}

		protected virtual object CreateLanguagePrimitive(Type nodeType, object source)
		{
			var node = (IElementNode)source;
			var result = nodeType != typeof(string) ? Activator.CreateInstance(nodeType) : string.Empty;
			if (node.CollectionItems.Count == 1 && node.CollectionItems.Single() is ValueNode && ((ValueNode)node.CollectionItems.Single()).Value is string)
			{
				string text = ((ValueNode)node.CollectionItems.Single()).Value as string;
				byte b;
				if (nodeType == typeof(bool))
				{
					bool flag;
					if (bool.TryParse(text, out flag))
					{
						result = flag;
					}
				}
				else if (nodeType == typeof(char))
				{
					char c;
					if (char.TryParse(text, out c))
					{
						result = c;
					}
				}
				else if (nodeType == typeof(string))
				{
					result = text;
				}
				else if (nodeType == typeof(decimal))
				{
					decimal num;
					if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out num))
					{
						result = num;
					}
				}
				else if (nodeType == typeof(float))
				{
					float num2;
					if (float.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out num2))
					{
						result = num2;
					}
				}
				else if (nodeType == typeof(double))
				{
					double num3;
					if (double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out num3))
					{
						result = num3;
					}
				}
				else if (nodeType == typeof(short))
				{
					short num4;
					if (short.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out num4))
					{
						result = num4;
					}
				}
				else if (nodeType == typeof(int))
				{
					int num5;
					if (int.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out num5))
					{
						result = num5;
					}
				}
				else if (nodeType == typeof(long))
				{
					long num6;
					if (long.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out num6))
					{
						result = num6;
					}
				}
				else if (nodeType == typeof(TimeSpan))
				{
					TimeSpan timeSpan;
					if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out timeSpan))
					{
						result = timeSpan;
					}
				}
				else if (nodeType == typeof(Uri))
				{
					Uri uri;
					if (Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out uri))
					{
						result = uri;
					}
				}
				else if (nodeType == typeof(byte) && byte.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out b))
				{
					result = b;
				}
			}
			return result;
		}
	}
}
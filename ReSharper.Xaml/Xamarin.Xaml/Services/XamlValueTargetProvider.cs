using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Xamarin.Xaml
{
	internal class XamlValueTargetProvider : IProvideParentValues, IProvideValueTarget
	{
		private INode Node
		{
			get;
			set;
		}
		private HydratationContext Context
		{
			get;
			set;
		}
		public object TargetObject
		{
			get;
			private set;
		}
		public object TargetProperty
		{
			get
			{
				throw new NotImplementedException();
			}
			private set
			{
			}
		}
		IEnumerable<object> IProvideParentValues.ParentObjects
		{
			get
			{
				if (this.Node != null && this.Context != null)
				{
					INode node = this.Node;
					object obj = null;
					HydratationContext hydratationContext = this.Context;
					while (node.Parent != null && hydratationContext != null)
					{
						if (node.Parent is IElementNode)
						{
							if (!hydratationContext.Values.TryGetValue(node.Parent, out obj))
							{
								hydratationContext = hydratationContext.ParentContext;
								continue;
							}
							yield return obj;
						}
						node = node.Parent;
					}
				}
				yield break;
			}
		}
		public XamlValueTargetProvider(object targetObject, INode node, HydratationContext context, object targetProperty)
		{
			this.Context = context;
			this.Node = node;
			this.TargetObject = targetObject;
			this.TargetProperty = targetProperty;
		}
	}
}

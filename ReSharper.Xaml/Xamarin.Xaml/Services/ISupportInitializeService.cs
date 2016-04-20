using System;

namespace Xamarin.Xaml
{
	public interface ISupportInitializeService
	{
		void BeginInitialize( object item );

		void EndInitialize( object item );
	}

	public class SupportInitializeService : ISupportInitializeService
	{
		public static SupportInitializeService Instance
		{
			get { return InstanceField; }
		}	static readonly SupportInitializeService InstanceField = new SupportInitializeService();

		public virtual void BeginInitialize( object item )
		{}

		public virtual void EndInitialize( object item )
		{}
	}
}
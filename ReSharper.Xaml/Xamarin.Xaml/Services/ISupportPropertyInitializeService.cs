namespace Xamarin.Xaml
{
	public interface ISupportPropertyInitializeService
	{
		object BeginSetProperty( object item, MemberDescriptor descriptor, object value );

		void EndSetProperty( object item, MemberDescriptor descriptor, object value );
	}

	public class SupportPropertyInitializeService : ISupportPropertyInitializeService
	{
		public static SupportPropertyInitializeService Instance
		{
			get { return InstanceField; }
		}	static readonly SupportPropertyInitializeService InstanceField = new SupportPropertyInitializeService();

		public virtual object BeginSetProperty( object item, MemberDescriptor descriptor, object value )
		{
			return value;
		}

		public virtual void EndSetProperty( object item, MemberDescriptor descriptor, object value )
		{}
	}
}
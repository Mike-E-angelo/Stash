using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace PostSharp.PassThrough
{
	static class Extensions
	{
		public static IEnumerable<ValueTuple<T1, T2>> Introduce<T1, T2>( this ImmutableArray<T1> @this, T2 instance ) => Introduce( @this, instance, tuple => true, t => t );

		//public static IEnumerable<T1> Introduce<T1, T2>( this IEnumerable<Func<T2, T1>> @this, T2 instance ) => Introduce( @this, instance, tuple => tuple.Item1( tuple.Item2 ) );

		public static IEnumerable<T1> Introduce<T1, T2>( this ImmutableArray<Func<T2, T1>> @this, T2 instance ) => Introduce( @this, instance, tuple => tuple.Item1( tuple.Item2 ) );

		public static IEnumerable<T1> Introduce<T1, T2>( this ImmutableArray<T1> @this, T2 instance, Func<ValueTuple<T1, T2>, bool> where ) => Introduce( @this, instance, @where, tuple => tuple.Item1 );

		public static IEnumerable<TResult> Introduce<T1, T2, TResult>( this ImmutableArray<T1> @this, T2 instance, Func<ValueTuple<T1, T2>, TResult> select ) => Introduce( @this, instance, tuple => true, @select );

		public static IEnumerable<TResult> Introduce<T1, T2, TResult>( this ImmutableArray<T1> @this, T2 instance, Func<ValueTuple<T1, T2>, bool> where, Func<ValueTuple<T1, T2>, TResult> select )
		{
			foreach ( var item in @this )
			{
				var tuple = ValueTuple.Create( item, instance );
				if ( where( tuple ) )
				{
					yield return select( tuple );
				}
			}
		}
	}

	// ATTRIBUTION: https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/InternalUtilities/ValueTuple.cs
	public static class ValueTuple
	{
		public static ValueTuple<T1, T2> Create<T1, T2>( T1 item1, T2 item2 ) => new ValueTuple<T1, T2>( item1, item2 );

		public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>( T1 item1, T2 item2, T3 item3 ) => new ValueTuple<T1, T2, T3>( item1, item2, item3 );
	}

	// ATTRIBUTION: https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/InternalUtilities/ValueTuple%603.cs
	public struct ValueTuple<T1, T2, T3> : IEquatable<ValueTuple<T1, T2, T3>>
	{
		readonly static EqualityComparer<T1> Comparer1 = EqualityComparer<T1>.Default;
		readonly static EqualityComparer<T2> Comparer2 = EqualityComparer<T2>.Default;
		readonly static EqualityComparer<T3> Comparer3 = EqualityComparer<T3>.Default;

		public ValueTuple( T1 item1, T2 item2, T3 item3 )
		{
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
		}

		public T1 Item1 { get; }
		public T2 Item2 { get; }
		public T3 Item3 { get; }

		public bool Equals( ValueTuple<T1, T2, T3> other )
		{
			return Comparer1.Equals( Item1, other.Item1 )
				   && Comparer2.Equals( Item2, other.Item2 )
				   && Comparer3.Equals( Item3, other.Item3 );
		}

		public override bool Equals( object obj )
		{
			if ( obj is ValueTuple<T1, T2, T3> )
			{
				var other = (ValueTuple<T1, T2, T3>)obj;
				return Equals( other );
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(
				Hash.Combine(
					Comparer1.GetHashCode( Item1 ),
					Comparer2.GetHashCode( Item2 ) ),
				Comparer3.GetHashCode( Item3 ) );
		}

		public static bool operator ==( ValueTuple<T1, T2, T3> left, ValueTuple<T1, T2, T3> right )
		{
			return left.Equals( right );
		}

		public static bool operator !=( ValueTuple<T1, T2, T3> left, ValueTuple<T1, T2, T3> right )
		{
			return !left.Equals( right );
		}
	}

	// ATTRIBUTION: https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/InternalUtilities/ValueTuple%602.cs
	public struct ValueTuple<T1, T2> : IEquatable<ValueTuple<T1, T2>>
	{
		readonly static EqualityComparer<T1> Comparer1 = EqualityComparer<T1>.Default;
		readonly static EqualityComparer<T2> Comparer2 = EqualityComparer<T2>.Default;

		public ValueTuple( T1 item1, T2 item2 )
		{
			Item1 = item1;
			Item2 = item2;
		}

		public T1 Item1 { get; }
		public T2 Item2 { get; }

		public bool Equals( ValueTuple<T1, T2> other ) => Comparer1.Equals( Item1, other.Item1 ) && Comparer2.Equals( Item2, other.Item2 );

		public override bool Equals( object obj )
		{
			if ( obj is ValueTuple<T1, T2> )
			{
				var other = (ValueTuple<T1, T2>)obj;
				return Equals( other );
			}

			return false;
		}

		public override int GetHashCode() => Hash.Combine( Comparer1.GetHashCode( Item1 ), Comparer2.GetHashCode( Item2 ) );

		public static bool operator ==( ValueTuple<T1, T2> left, ValueTuple<T1, T2> right ) => left.Equals( right );

		public static bool operator !=( ValueTuple<T1, T2> left, ValueTuple<T1, T2> right ) => !left.Equals( right );
	}

	static class Hash
	{
		/// <summary>
		///     This is how VB Anonymous Types combine hash values for fields.
		/// </summary>
		internal static int Combine( int newKey, int currentKey ) => unchecked( currentKey * (int)0xA5555529 + newKey );

		internal static int Combine( bool newKeyPart, int currentKey ) => Combine( currentKey, newKeyPart ? 1 : 0 );

		/// <summary>
		///     This is how VB Anonymous Types combine hash values for fields.
		///     PERF: Do not use with enum types because that involves multiple
		///     unnecessary boxing operations.  Unfortunately, we can't constrain
		///     T to "non-enum", so we'll use a more restrictive constraint.
		/// </summary>
		internal static int Combine<T>( T newKeyPart, int currentKey ) where T : class
		{
			var hash = unchecked( currentKey * (int)0xA5555529 );

			if ( newKeyPart != null )
				return unchecked( hash + newKeyPart.GetHashCode() );

			return hash;
		}

		internal static int CombineValues<T>( IEnumerable<T> values, int maxItemsToHash = int.MaxValue )
		{
			if ( values == null )
				return 0;

			var hashCode = 0;
			var count = 0;
			foreach ( var value in values )
			{
				if ( count++ >= maxItemsToHash )
					break;

				// Should end up with a constrained virtual call to object.GetHashCode (i.e. avoid boxing where possible).
				if ( value != null )
					hashCode = Combine( value.GetHashCode(), hashCode );
			}

			return hashCode;
		}

		internal static int CombineValues<T>( T[] values, int maxItemsToHash = int.MaxValue )
		{
			if ( values == null )
				return 0;

			var maxSize = Math.Min( maxItemsToHash, values.Length );
			var hashCode = 0;

			for ( var i = 0; i < maxSize; i++ )
			{
				var value = values[i];

				// Should end up with a constrained virtual call to object.GetHashCode (i.e. avoid boxing where possible).
				if ( value != null )
					hashCode = Combine( value.GetHashCode(), hashCode );
			}

			return hashCode;
		}

		internal static int CombineValues<T>( ImmutableArray<T> values, int maxItemsToHash = int.MaxValue )
		{
			if ( values.IsDefaultOrEmpty )
				return 0;

			var hashCode = 0;
			var count = 0;
			foreach ( var value in values )
			{
				if ( count++ >= maxItemsToHash )
					break;

				// Should end up with a constrained virtual call to object.GetHashCode (i.e. avoid boxing where possible).
				if ( value != null )
					hashCode = Combine( value.GetHashCode(), hashCode );
			}

			return hashCode;
		}

		internal static int CombineValues( IEnumerable<string> values, StringComparer stringComparer, int maxItemsToHash = int.MaxValue )
		{
			if ( values == null )
				return 0;

			var hashCode = 0;
			var count = 0;
			foreach ( var value in values )
			{
				if ( count++ >= maxItemsToHash )
					break;

				if ( value != null )
					hashCode = Combine( stringComparer.GetHashCode( value ), hashCode );
			}

			return hashCode;
		}

		/// <summary>
		///     The offset bias value used in the FNV-1a algorithm
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		const int FnvOffsetBias = unchecked( (int)2166136261 );

		/// <summary>
		///     The generative factor used in the FNV-1a algorithm
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		const int FnvPrime = 16777619;

		/// <summary>
		///     Compute the FNV-1a hash of a sequence of bytes
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="data">The sequence of bytes</param>
		/// <returns>The FNV-1a hash of <paramref name="data" /></returns>
		internal static int GetFnvHashCode( byte[] data )
		{
			var hashCode = FnvOffsetBias;

			for ( var i = 0; i < data.Length; i++ )
				hashCode = unchecked( ( hashCode ^ data[i] ) * FnvPrime );

			return hashCode;
		}

		/*/// <summary>
			/// Compute the FNV-1a hash of a sequence of bytes and determines if the byte
			/// sequence is valid ASCII and hence the hash code matches a char sequence
			/// encoding the same text.
			/// See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
			/// </summary>
			/// <param name="data">The sequence of bytes that are likely to be ASCII text.</param>
			/// <param name="length">The length of the sequence.</param>
			/// <param name="isAscii">True if the sequence contains only characters in the ASCII range.</param>
			/// <returns>The FNV-1a hash of <paramref name="data"/></returns>
			internal static unsafe int GetFNVHashCode( byte* data, int length, out bool isAscii )
			{
				int hashCode = Hash.FnvOffsetBias;

				byte asciiMask = 0;

				for ( int i = 0; i < length; i++ )
				{
					byte b = data[i];
					asciiMask |= b;
					hashCode = unchecked( ( hashCode ^ b ) * Hash.FnvPrime );
				}

				isAscii = ( asciiMask & 0x80 ) == 0;
				return hashCode;
			}*/

		/// <summary>
		///     Compute the FNV-1a hash of a sequence of bytes
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="data">The sequence of bytes</param>
		/// <returns>The FNV-1a hash of <paramref name="data" /></returns>
		internal static int GetFnvHashCode( ImmutableArray<byte> data )
		{
			var hashCode = FnvOffsetBias;

			for ( var i = 0; i < data.Length; i++ )
				hashCode = unchecked( ( hashCode ^ data[i] ) * FnvPrime );

			return hashCode;
		}

		/// <summary>
		///     Compute the hashcode of a sub-string using FNV-1a
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		///     Note: FNV-1a was developed and tuned for 8-bit sequences. We're using it here
		///     for 16-bit Unicode chars on the understanding that the majority of chars will
		///     fit into 8-bits and, therefore, the algorithm will retain its desirable traits
		///     for generating hash codes.
		/// </summary>
		/// <param name="text">The input string</param>
		/// <param name="start">The start index of the first character to hash</param>
		/// <param name="length">The number of characters, beginning with <paramref name="start" /> to hash</param>
		/// <returns>
		///     The FNV-1a hash code of the substring beginning at <paramref name="start" /> and ending after
		///     <paramref name="length" /> characters.
		/// </returns>
		internal static int GetFnvHashCode( string text, int start, int length )
		{
			var hashCode = FnvOffsetBias;
			var end = start + length;

			for ( var i = start; i < end; i++ )
				hashCode = unchecked( ( hashCode ^ text[i] ) * FnvPrime );

			return hashCode;
		}

		/// <summary>
		///     Compute the hashcode of a sub-string using FNV-1a
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="text">The input string</param>
		/// <param name="start">The start index of the first character to hash</param>
		/// <returns>
		///     The FNV-1a hash code of the substring beginning at <paramref name="start" /> and ending at the end of the
		///     string.
		/// </returns>
		internal static int GetFnvHashCode( string text, int start ) => GetFnvHashCode( text, start, text.Length - start );

		/// <summary>
		///     Compute the hashcode of a string using FNV-1a
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="text">The input string</param>
		/// <returns>The FNV-1a hash code of <paramref name="text" /></returns>
		internal static int GetFnvHashCode( string text ) => CombineFnvHash( FnvOffsetBias, text );

		/// <summary>
		///     Compute the hashcode of a string using FNV-1a
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="text">The input string</param>
		/// <returns>The FNV-1a hash code of <paramref name="text" /></returns>
		internal static int GetFnvHashCode( StringBuilder text )
		{
			var hashCode = FnvOffsetBias;
			var end = text.Length;

			for ( var i = 0; i < end; i++ )
				hashCode = unchecked( ( hashCode ^ text[i] ) * FnvPrime );

			return hashCode;
		}

		/// <summary>
		///     Compute the hashcode of a sub string using FNV-1a
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="text">The input string as a char array</param>
		/// <param name="start">The start index of the first character to hash</param>
		/// <param name="length">The number of characters, beginning with <paramref name="start" /> to hash</param>
		/// <returns>
		///     The FNV-1a hash code of the substring beginning at <paramref name="start" /> and ending after
		///     <paramref name="length" /> characters.
		/// </returns>
		internal static int GetFnvHashCode( char[] text, int start, int length )
		{
			var hashCode = FnvOffsetBias;
			var end = start + length;

			for ( var i = start; i < end; i++ )
				hashCode = unchecked( ( hashCode ^ text[i] ) * FnvPrime );

			return hashCode;
		}

		/// <summary>
		///     Compute the hashcode of a single character using the FNV-1a algorithm
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		///     Note: In general, this isn't any more useful than "char.GetHashCode". However,
		///     it may be needed if you need to generate the same hash code as a string or
		///     substring with just a single character.
		/// </summary>
		/// <param name="ch">The character to hash</param>
		/// <returns>The FNV-1a hash code of the character.</returns>
		internal static int GetFnvHashCode( char ch ) => CombineFnvHash( FnvOffsetBias, ch );

		/// <summary>
		///     Combine a string with an existing FNV-1a hash code
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="hashCode">The accumulated hash code</param>
		/// <param name="text">The string to combine</param>
		/// <returns>The result of combining <paramref name="hashCode" /> with <paramref name="text" /> using the FNV-1a algorithm</returns>
		internal static int CombineFnvHash( int hashCode, string text )
		{
			foreach ( var ch in text )
				hashCode = unchecked( ( hashCode ^ ch ) * FnvPrime );

			return hashCode;
		}

		/// <summary>
		///     Combine a char with an existing FNV-1a hash code
		///     See http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="hashCode">The accumulated hash code</param>
		/// <param name="ch">The new character to combine</param>
		/// <returns>The result of combining <paramref name="hashCode" /> with <paramref name="ch" /> using the FNV-1a algorithm</returns>
		internal static int CombineFnvHash( int hashCode, char ch ) => unchecked( ( hashCode ^ ch ) * FnvPrime );
	}
}

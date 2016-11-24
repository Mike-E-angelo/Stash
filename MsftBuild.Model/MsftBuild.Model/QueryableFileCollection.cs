using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MsftBuild.Model
{
	public interface IList<out T> : IEnumerable<T>, IList {}

	public class Collection<T> : System.Collections.ObjectModel.Collection<T>, IList<T> {}

	public class List<T> : IList<T>
	{
		// readonly System.Collections.ObjectModel.Collection<IFile> collection;
		readonly IList collection;

		public List() : this( new System.Collections.Generic.List<IFile>() ) {}

		public List( IList collection )
		{
			this.collection = collection;
		}

		public virtual IEnumerator<T> GetEnumerator() => collection.OfType<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();

		public void CopyTo( Array array, int index ) => collection.CopyTo( array, index );

		public int Count => collection.Count;

		public object SyncRoot => collection.SyncRoot;

		public bool IsSynchronized => collection.IsSynchronized;

		public int Add( object value ) => collection.Add( value );

		public bool Contains( object value ) => collection.Contains( value );

		public void Clear() => collection.Clear();

		public int IndexOf( object value ) => collection.IndexOf( value );

		public void Insert( int index, object value ) => collection.Insert( index, value );

		public void Remove( object value ) => collection.Remove( value );

		public void RemoveAt( int index ) => collection.RemoveAt( index );

		public object this[ int index ]
		{
			get { return collection[index]; }
			set { collection[index] = value; }
		}

		public bool IsReadOnly => collection.IsReadOnly;

		public bool IsFixedSize => collection.IsFixedSize;
	}

	public interface IFileList : IList<IFile> {}

	public class FileList : Collection<IFile>, IFileList { }

	public class QueryableFileCollection : List<IFile>, IFileList
	{
		readonly static string RecursivePattern = string.Concat( "**", Path.DirectorySeparatorChar );

		public string Query { get; set; }

		public override IEnumerator<IFile> GetEnumerator()
		{
			var recursive = Query.Contains( RecursivePattern );
			var query = recursive ? Query.Replace( RecursivePattern, string.Empty ) : Query;
			var result =
				new DirectoryInfo( Directory.GetCurrentDirectory() ) 
					.GetFiles( query, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly )
					.Select( info => new File( info.FullName ) )
					.GetEnumerator();
			return result;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace BenchmarkDotNet.Gc
{
	class Program
	{
		// ReSharper disable once UnusedMember.Local
		static void Main(string[] args) => new BenchmarkSwitcher(new[]
		                                                         {
			                                                         typeof(TroubleshootingSlow),
			                                                         typeof(TroubleshootingFast),
		                                                         }).Run(args);
	}

	[Config(typeof(TroubleshootConfiguration))]
	public class TroubleshootingFast
	{
		public TroubleshootingFast()
		{

			for (int i = 0; i < 10000; i++)
			{
				Enumerable.Repeat(new object(), 1000)
				          .Select(x => new object())
				          .ToArray();
			}

			Fast();
		}

		[Benchmark]
		public string Fast()
		{
			using (var stream = new MemoryStream())
			{
				var xml = XmlWriter.Create(stream);
				using (new SlowWriter(null, null, null, null, xml))
				{
					xml.WriteStartElement("int");
					xml.WriteString("6776");
					xml.WriteEndElement();
				}
				stream.Seek(0, SeekOrigin.Begin);
				var result = new StreamReader(stream).ReadToEnd();
				return result;
			}
		}
	}

	[Config(typeof(TroubleshootConfiguration))]
	public class TroubleshootingSlow
	{
		public TroubleshootingSlow()
		{
			Slow();
		}

		[Benchmark]
		public string Slow()
		{
			using (var stream = new MemoryStream())
			{
				var xml = XmlWriter.Create(stream);
				using (new SlowWriter(null, null, null, null, xml))
				{
					xml.WriteStartElement("int");
					xml.WriteString("6776");
					xml.WriteEndElement();
				}
				stream.Seek(0, SeekOrigin.Begin);
				var result = new StreamReader(stream).ReadToEnd();
				return result;
			}
		}
	}


	sealed class SlowWriter : Dictionary<string, string>, IDisposable
	{
		public interface IParameterizedSource<in TParameter, out TResult>
		{
			TResult Get(TParameter parameter);
		}

		public interface IFormatter<in T> : IParameterizedSource<T, string> { }

		public interface IAlteration<T> : IParameterizedSource<T, T> { }

		public interface IAliases : IAlteration<string> { }

		public interface IIdentifierFormatter : IFormatter<int> { }

		public interface IIdentityStore
		{
			IIdentity Get(string name, string identifier);
		}

		public interface IIdentity
		{
			string Identifier { get; }
			string Name { get; }
		}

		public interface ITypePartResolver : IParameterizedSource<TypeInfo, TypeParts> { }

		public struct TypeParts : IIdentity
		{
			readonly Func<ImmutableArray<TypeParts>> _arguments;

			public TypeParts(string name, string identifier = "", Func<ImmutableArray<TypeParts>> arguments = null)
			{
				Name = name;
				Identifier = identifier;
				_arguments = arguments;
			}

			public string Identifier { get; }
			public string Name { get; }

			public ImmutableArray<TypeParts>? GetArguments() => _arguments?.Invoke();
		}


		/*readonly static Delimiter Separator = DefaultClrDelimiters.Default.Separator;*/
		readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;

		readonly IAliases _aliases;
		readonly IIdentifierFormatter _formatter;
		readonly IIdentityStore _store;
		readonly ITypePartResolver _parts;

		readonly System.Xml.XmlWriter _writer;
		readonly Func<TypeParts, TypeParts> _selector;

		public SlowWriter(IAliases aliases, IIdentifierFormatter formatter, IIdentityStore store, ITypePartResolver parts,
		                  System.Xml.XmlWriter writer)
		{
			_aliases = aliases;
			_formatter = formatter;
			_store = store;
			_parts = parts;
			_writer = writer;
			_selector = Get;
		}

		static TypeParts Get(TypeParts parameter) => default(TypeParts);

		public void Dispose()
		{
			_writer.Dispose();
			Clear();
		}
	}


	sealed class TroubleshootConfiguration : ManualConfig
	{
		public TroubleshootConfiguration()
		{
			Add(Job.Default
			       .WithWarmupCount(5)
			       .WithTargetCount(10));
		}
	}

	/*
		class Program
		{
			static void Main(string[] args) => new BenchmarkSwitcher(new[]
																	 {
																		 typeof(TroubleshootingSlow),
																		 typeof(TroubleshootingFast)
																	 }).Run(args);
		}

		[Config(typeof(TroubleshootConfiguration))]
		public class TroubleshootingSlow
		{
			public TroubleshootingSlow()
			{
				for (int i = 0; i < 10000; i++)
				{
					Enumerable.Repeat(new object(), 1000)
							  .Select(x => new object())
							  .ToArray();
				}

				Slow();
			}

			[Benchmark]
			public string Slow()
			{
				using (var stream = new MemoryStream())
				{
					var xml = XmlWriter.Create(stream);
					using (new SlowWriter(null, null, null, null, xml))
					{
						xml.WriteStartElement("int");
						xml.WriteString("6776");
						xml.WriteEndElement();
					}
					stream.Seek(0, SeekOrigin.Begin);
					var result = new StreamReader(stream).ReadToEnd();
					return result;
				}
			}
		}

		[Config(typeof(TroubleshootConfiguration))]
		public class TroubleshootingFast
		{
			public TroubleshootingFast()
			{
				Fast();
			}

			[Benchmark]
			public string Fast()
			{
				using (var stream = new MemoryStream())
				{
					var xml = XmlWriter.Create(stream);
					using (new SlowWriter(null, null, null, null, xml))
					{
						xml.WriteStartElement("int");
						xml.WriteString("6776");
						xml.WriteEndElement();
					}
					stream.Seek(0, SeekOrigin.Begin);
					var result = new StreamReader(stream).ReadToEnd();
					return result;
				}
			}
		}

		sealed class SlowWriter : Dictionary<string, string>, IDisposable
		{
			public interface IParameterizedSource<in TParameter, out TResult>
			{
				TResult Get(TParameter parameter);
			}

			public interface IFormatter<in T> : IParameterizedSource<T, string> { }

			public interface IAlteration<T> : IParameterizedSource<T, T> { }

			public interface IAliases : IAlteration<string> { }

			public interface IIdentifierFormatter : IFormatter<int> { }

			public interface IIdentityStore
			{
				IIdentity Get(string name, string identifier);
			}

			public interface IIdentity
			{
				string Identifier { get; }
				string Name { get; }
			}

			public interface ITypePartResolver : IParameterizedSource<TypeInfo, TypeParts> { }

			public struct TypeParts : IIdentity
			{
				readonly Func<ImmutableArray<TypeParts>> _arguments;

				public TypeParts(string name, string identifier = "", Func<ImmutableArray<TypeParts>> arguments = null)
				{
					Name = name;
					Identifier = identifier;
					_arguments = arguments;
				}

				public string Identifier { get; }
				public string Name { get; }

				public ImmutableArray<TypeParts>? GetArguments() => _arguments?.Invoke();
			}


			/*readonly static Delimiter Separator = DefaultClrDelimiters.Default.Separator;#1#
			readonly static string Xmlns = XNamespace.Xmlns.NamespaceName;

			readonly IAliases _aliases;
			readonly IIdentifierFormatter _formatter;
			readonly IIdentityStore _store;
			readonly ITypePartResolver _parts;

			readonly System.Xml.XmlWriter _writer;
			readonly Func<TypeParts, TypeParts> _selector;

			public SlowWriter(IAliases aliases, IIdentifierFormatter formatter, IIdentityStore store, ITypePartResolver parts,
							  System.Xml.XmlWriter writer)
			{
				_aliases = aliases;
				_formatter = formatter;
				_store = store;
				_parts = parts;
				_writer = writer;
				_selector = Get;
			}

			static TypeParts Get(TypeParts parameter) => default(TypeParts);

			public void Dispose()
			{
				_writer.Dispose();
				Clear();
			}
		}


		sealed class TroubleshootConfiguration : ManualConfig
		{
			public TroubleshootConfiguration()
			{
				Add(Job.Default
					   .WithWarmupCount(5)
					   .WithTargetCount(10));
			}
		}
	*/
}
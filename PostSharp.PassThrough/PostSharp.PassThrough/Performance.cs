using System;
using System.Collections.Immutable;
using System.Linq;

namespace PostSharp.PassThrough
{
	public interface IParameterizedSource<out T> : IParameterizedSource<object, T> {}

	public interface IParameterizedSource<in TParameter, out TResult>
	{
		TResult Get( TParameter parameter );
	}

	public abstract class ParameterizedSourceBase<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		public abstract TResult Get( TParameter parameter );
	}

	public class PerformanceSupport
	{
		readonly static string[] Titles = { "Test", "Average", "Median", "Mode" };
		const string TimeFormat = "ss'.'ffff";

		readonly Action<string> output;
		readonly ImmutableArray<Action> actions;

		public PerformanceSupport( Action<string> output, params Action[] actions )
		{
			this.output = output;
			this.actions = actions.ToImmutableArray();

			foreach ( var action in actions )
			{
				action();
			}
		}

		public void Run( int numberOfRuns = 100, int perRun = 10000 )
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();

			var results = ReportSource.Default.Get( new ReportSource.Parameter( actions, numberOfRuns, perRun ) ).ToArray();

			var max = results.Max( r => r.Name.Length );
			var template = $"{{0,-{max}}} | {{1, 7}} | {{2, 7}} | {{3, 7}}";

			var title = string.Format( template, Titles );
			output( title );
			output( new string( '-', title.Length ) );

			foreach ( var result in results )
			{
				output( string.Format( template, result.Name, result.Average.ToString( TimeFormat ), result.Median.ToString( TimeFormat ), result.Mode.ToString( TimeFormat ) ) );
			}
		}
	}

	class ReportSource : ParameterizedSourceBase<ReportSource.Parameter, ImmutableArray<ReportSource.Result>>
	{
		public static ReportSource Default { get; } = new ReportSource();

		public override ImmutableArray<Result> Get( Parameter parameter ) => 
			parameter.Actions.Introduce( parameter, tuple => new Run( tuple.Item1, tuple.Item2.NumberOfRuns, tuple.Item2.PerRun ).Get() ).ToImmutableArray();

		public struct Parameter
		{
			public Parameter( ImmutableArray<Action> actions, int numberOfRuns = 100, int perRun = 10000 )
			{
				Actions = actions;
				NumberOfRuns = numberOfRuns;
				PerRun = perRun;
			}

			public ImmutableArray<Action> Actions { get; }
			public int NumberOfRuns { get; }
			public int PerRun { get; }
		}

		public struct Result
		{
			public Result( string name, TimeSpan average, TimeSpan median, TimeSpan mode )
			{
				Name = name;
				Average = average;
				Median = median;
				Mode = mode;
			}

			public string Name { get; }
			public TimeSpan Average { get; }
			public TimeSpan Median { get; }
			public TimeSpan Mode { get; }
		}
	}

	public interface ISource<out T> : ISource
	{
		new T Get();
	}

	public interface ISource
	{
		object Get();
	}

	public abstract class SourceBase<T> : ISource<T>
	{
		public abstract T Get();

		object ISource.Get() => Get();
		
	}

	sealed class Run : SourceBase<ReportSource.Result>
	{
		readonly Action action;
		readonly int numberOfSamples;
		readonly int perSample;
				
		public Run( Action action, int numberOfSamples, int perSample )
		{
			this.action = action;
			this.numberOfSamples = numberOfSamples;
			this.perSample = perSample;
		}

		public override ReportSource.Result Get()
		{
			var data = EnumerableEx.Generate( 0, Continue, i => i + 1, Measure ).Select( span => span.Ticks ).ToArray();
			var average = data.Average( span => span );
			var median = MedianFactory.Default.Get( data.ToImmutableArray() );
			var mode = ModeFactory<long>.Default.Get( data.ToImmutableArray() );
			var result = new ReportSource.Result( action.Method.Name, TimeSpan.FromTicks( (long)average ), TimeSpan.FromTicks( median ), TimeSpan.FromTicks( mode ) );
			return result;
		}

		bool Continue( int i ) => i < numberOfSamples;

		TimeSpan Measure<T>( T _ )
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			for ( var i = 0; i < perSample; i++ )
			{
				action();
			}
			watch.Stop();
			var result = watch.Elapsed;
			return result;
		}
	}

	sealed class MedianFactory : ParameterizedSourceBase<ImmutableArray<long>, long>
	{
		public static MedianFactory Default { get; } = new MedianFactory();

		public override long Get( ImmutableArray<long> parameter )
		{
			var length = parameter.Length;
			var middle = length / 2;
			var ordered = parameter.ToArray().OrderBy( i => i ).ToArray();
			var median = ordered.ElementAt( middle ) + ordered.ElementAt( ( length - 1 ) / 2 );
			var result = median / 2;
			return result;
		}
	}

	sealed class ModeFactory<T> : ParameterizedSourceBase<ImmutableArray<T>, T>
	{
		public static ModeFactory<T> Default { get; } = new ModeFactory<T>();
		public override T Get( ImmutableArray<T> parameter ) => parameter.ToArray().GroupBy( n => n ).OrderByDescending( g => g.Count() ).Select( g => g.Key ).FirstOrDefault();
	}
}

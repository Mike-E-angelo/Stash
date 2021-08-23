using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace EfCore.ScopedVsTransaction
{
	sealed class Program
	{
		static void Main(string[] args)
		{
			var config = DefaultConfig.Instance.AddJob(Job.InProcessDontLogOutput)
			                          .WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend))
			                          .AddDiagnoser(MemoryDiagnoser.Default)
			                          .AddExporter(MarkdownExporter.GitHub);
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
		}
	}
}
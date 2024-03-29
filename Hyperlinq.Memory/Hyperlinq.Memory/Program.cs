﻿using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Hyperlinq.Memory
{
	public class Program
	{
		static void Main(string[] args)
		{
			var config = DefaultConfig.Instance
			                          .WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend))
			                          .AddDiagnoser(MemoryDiagnoser.Default)
			                          .AddExporter(MarkdownExporter.GitHub);
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
		}

		
	}
}
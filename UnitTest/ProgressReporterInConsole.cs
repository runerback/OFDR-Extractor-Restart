using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.UnitTest
{
	sealed class ProgressReporterInConsole : IProgressReporter
	{
		public ProgressReporterInConsole(bool report = true)
		{
			this.report = report;
		}

		private bool report;

		public void Report(string status)
		{
			if(report)
				Console.WriteLine(status);
		}

		public void Report(double percent)
		{
			if (report)
				Console.WriteLine("{0:0.00}%", percent * 100);
		}

		public void Report(double percent, string status)
		{
			if (report)
				Console.WriteLine("{0:0.00}%: {1}", percent * 100, status);
		}

		public void Start(string status)
		{
			if (report)
				Report(0, status);
		}

		public void Complete(string status)
		{
			if (report)
				Report(1, status);
		}
	}
}

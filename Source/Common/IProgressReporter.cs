using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor
{
	public interface IProgressReporter
	{
		void Report(string status);
		void Report(double percent);
		void Report(double percent, string status);

		void Start(string status);
		void Complete(string status);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface IProgressReporterController
	{
		double Progress { get; }
		string Status { get; }
	}
}

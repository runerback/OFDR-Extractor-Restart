﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface IProgressReporter
	{
		void Report(string status);
		void Report(double percent);
	}
}

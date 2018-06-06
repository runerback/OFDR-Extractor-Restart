using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OFDRExtractor.GUI
{
	sealed class ShellViewModel : ViewModelBase
	{
		private ShellViewModel() { }

		public static readonly ShellViewModel Instance = new ShellViewModel();

		private readonly ProgressReporterController progressReporter = 
			ProgressReporterController.Instance;
		public ProgressReporterController ProgressReporter
		{
			get { return this.progressReporter; }
		}
	}
}

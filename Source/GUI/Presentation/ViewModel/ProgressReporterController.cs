using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	sealed class ProgressReporterController : ViewModelBase, IProgressReporterController, IProgressReporter
	{
		private ProgressReporterController() { }

		private static readonly ProgressReporterController instance = new ProgressReporterController();
		public static ProgressReporterController Instance
		{
			get { return instance; }
		}

		#region IProgressReporterController
		
		private double progress = 0;
		public double Progress
		{
			get { return this.progress; }
		}

		private void setProgress(double value)
		{
			if (this.progress != value)
			{
				this.progress = value;
				NotifyPropertyChanged("Progress");
			}
		}

		private string status = null;
		public string Status
		{
			get { return this.status; }
		}

		private void setStatus(string value)
		{
			if (this.status != value)
			{
				this.status = value;
				NotifyPropertyChanged("Status");
			}
		}

		#endregion IProgressReporterController

		#region IProgressReporter

		void IProgressReporter.Report(string status)
		{
			setStatus(status);
		}

		void IProgressReporter.Report(double percent)
		{
			setProgress(percent);
		}

		void IProgressReporter.Report(double percent, string status)
		{
			setProgress(percent);
			setStatus(status);
		}

		void IProgressReporter.Start(string status)
		{
			((IProgressReporter)this).Report(0, status);
		}

		void IProgressReporter.Complete(string status)
		{
			((IProgressReporter)this).Report(1, status);
		}

		#endregion IProgressReporter

	}
}

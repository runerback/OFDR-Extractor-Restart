using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	sealed class ProgressReporterController : ViewModelBase, IProgressReporter
	{
		private ProgressReporterController() { }
		
		private static readonly ProgressReporterController instance = new ProgressReporterController();
		public static ProgressReporterController Instance
		{
			get { return instance; }
		}

		private double progress = 0;
		public double Progress
		{
			get { return this.progress; }
			private set
			{
				if (this.progress != value)
				{
					this.progress = value;
					NotifyPropertyChanged("Progress");
				}
			}
		}

		private string status = null;
		public string Status
		{
			get { return this.status; }
			private set
			{
				if (this.status != value)
				{
					this.status = value;
					NotifyPropertyChanged("Status");
				}
			}
		}

		#region IProgressReporter

		void IProgressReporter.Report(string status)
		{
			this.Status = status;
		}

		void IProgressReporter.Report(double percent)
		{
			this.Progress = percent;
		}

		void IProgressReporter.Report(double percent, string status)
		{
			this.Progress = percent;
			this.Status = status;
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

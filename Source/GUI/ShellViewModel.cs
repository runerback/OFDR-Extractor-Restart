using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OFDRExtractor.GUI
{
	sealed class ShellViewModel : INotifyPropertyChanged, IProgressReporter
	{
		private ShellViewModel() { }

		public static readonly ShellViewModel Instance = new ShellViewModel();
		
		#region Properties

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

		#endregion Properties

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged

		#region IProgressReporter

		void IProgressReporter.Report(string status)
		{
			this.Status = status;
		}

		void IProgressReporter.Report(double percent)
		{
			this.Progress = percent * 1000;
		}

		void IProgressReporter.Report(double percent, string status)
		{
			this.Progress = percent * 1000;
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

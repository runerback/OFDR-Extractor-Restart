using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OFDRExtractor.GUI.Presentation.ViewModel;

namespace OFDRExtractor.GUI
{
	sealed class ShellViewModel : ViewModelBase
	{
		private ShellViewModel()
		{
			this.fileDataController.ToolBarController.RefreshCommand.Execute(null);
		}

		public static readonly ShellViewModel Instance = new ShellViewModel();

		private readonly IProgressReporterController progressReporterController = 
			ProgressReporterController.Instance;
		public IProgressReporterController ProgressReporter
		{
			get { return this.progressReporterController; }
		}

		private readonly IBusyLayerController busyLayerController = BusyLayerViewModel.Instance;
		public IBusyLayerController BusyLayer
		{
			get { return this.busyLayerController; }
		}

		private readonly IFileDataController fileDataController = new FileDataViewModel();
		public IFileDataController FileData
		{
			get { return this.fileDataController; }
		}
	}
}

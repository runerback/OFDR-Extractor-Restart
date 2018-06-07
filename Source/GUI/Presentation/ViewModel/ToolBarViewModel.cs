using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Presentation.ViewModel
{
	sealed class ToolBarViewModel : ViewModelBase, IToolBarController
	{
		private ToolBarViewModel()
		{
			this.refreshCommand = new SimpleCommand(refresh);
			this.unpackAllCommand = new SimpleCommand(unpackAll);
			this.unpackSelectionCommand = new DelegateCommand(unpackSelection, canUnpackSelection);
		}

		private readonly IBusyLayerController busyLayer = BusyLayerViewModel.Instance;
		private readonly IProgressReporter reporter = ProgressReporterController.Instance;

		public ToolBarViewModel(IFileDataProvider fileDataProvider)
			: this()
		{
			if (fileDataProvider == null)
				throw new ArgumentNullException("fileDataProvider");
			this.fileDataProvider = fileDataProvider;
			fileDataProvider.SelectedFilesCountChanged += onSelectedFilesCountChanged;
		}

		private readonly IFileDataProvider fileDataProvider;

		private void onSelectedFilesCountChanged(object sender, EventArgs e)
		{
			this.selectedFilesCount = this.fileDataProvider.SelectedFilesCount;
			NotifyPropertyChanged("SelectedFilesCount");
			this.unpackSelectionCommand.NotifyCanExecuteChanged();
		}

		private int selectedFilesCount = 0;
		public int SelectedFilesCount
		{
			get { return this.selectedFilesCount; }
		}

		#region refresh

		private SimpleCommand refreshCommand;
		public System.Windows.Input.ICommand RefreshCommand
		{
			get { return this.refreshCommand; }
		}

		private void refresh(object obj)
		{
			this.busyLayer.Busy();
			OFDRUnpacker.Load(this.reporter)
				.ContinueWith(t =>
				{
					this.busyLayer.Idle();
					raiseFolderRootBuilt(t.Result);
				});
		}

		public event EventHandler<FolderRootBuiltEventArgs> FolderRootBuilt;
		private void raiseFolderRootBuilt(Model.FolderData root)
		{
			if (FolderRootBuilt != null)
				FolderRootBuilt(this, new FolderRootBuiltEventArgs(root));
		}

		#endregion refresh

		#region unpack
		
		private SimpleCommand unpackAllCommand;
		public System.Windows.Input.ICommand UnpackAllCommand
		{
			get { return this.unpackAllCommand; }
		}

		private void unpackAll(object obj)
		{
			unpack(this.fileDataProvider.AllFiles);
		}

		private DelegateCommand unpackSelectionCommand;
		public System.Windows.Input.ICommand UnpackSelectionCommand
		{
			get { return this.unpackSelectionCommand; }
		}

		private bool canUnpackSelection(object obj)
		{
			return this.selectedFilesCount > 0;
		}

		private void unpackSelection(object obj)
		{
			unpack(this.fileDataProvider.SelectedFiles);
		}

		private void unpack(IEnumerable<Model.FileData> files)
		{
			this.busyLayer.Busy();
			OFDRUnpacker.Unpack(this.reporter, files)
				.ContinueWith(t =>
				{
					this.busyLayer.Idle();
					raiseUnpacked();
				});
		}

		public event EventHandler Unpacked;
		private void raiseUnpacked()
		{
			if (Unpacked != null)
				Unpacked(this, EventArgs.Empty);
		}

		#endregion unpack

	}

	sealed class FolderRootBuiltEventArgs : EventArgs
	{
		public FolderRootBuiltEventArgs(Model.FolderData root)
		{
			if (root == null)
				throw new ArgumentNullException("root");
			this.root = root;
		}

		private Model.FolderData root;
		public Model.FolderData Root
		{
			get { return this.root; }
		}
	}
}

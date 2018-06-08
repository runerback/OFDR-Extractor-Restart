using OFDRExtractor.GUI.Business;
using OFDRExtractor.GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Presentation.ViewModel
{
	sealed class FileDataViewModel : ViewModelBase, IFileDataController, IFileDataProvider
	{
		public FileDataViewModel()
		{
			var toolbar = new ToolBarViewModel(this);
			toolbar.FolderRootBuilt += onFolderRootBuilt;
			toolbar.Unpacked += onUnpackCompleted;

			this.toolbar = toolbar;
		}

		#region toolbar

		private readonly IToolBarController toolbar;
		public IToolBarController ToolBarController
		{
			get { return this.toolbar; }
		}

		private void onFolderRootBuilt(object sender, FolderRootBuiltEventArgs e)
		{
			onFolderRootChanged(e.Root);
		}

		private void onUnpackCompleted(object sender, EventArgs e)
		{

		}

		#endregion toolbar

		private FolderData root = null;
		public IEnumerable<FolderData> RootSource
		{
			get
			{
				if (this.root == null)
					yield break;
				yield return this.root;
			}
		}

		private SelectableManager selectableManager;

		private void onFolderRootChanged(FolderData root)
		{
			if (this.selectableManager != null)
			{
				this.selectableManager.SelectedFileCountChanged -= onSelectedFileCountChanged;
				this.selectableManager.Dispose();
				this.selectableManager = null;
			}

			this.root = root;
			NotifyPropertyChanged("RootSource");

			if (root != null)
			{
				this.selectableManager = new SelectableManager(root);
				this.selectableManager.SelectedFileCountChanged += onSelectedFileCountChanged;
			}
		}

		private void onSelectedFileCountChanged(object sender, EventArgs e)
		{
			this.selectedFilesCount = this.selectableManager.SelectedFileCount;
			raiseSelectedFilesCountChanged();
		}

		#region IFileDataProvider
		
		IEnumerable<FileData> IFileDataProvider.AllFiles
		{
			get
			{
				if (this.root == null)
					return Enumerable.Empty<FileData>();
				return new FileDataEnumerable(this.root);
			}
		}

		IEnumerable<FileData> IFileDataProvider.SelectedFiles
		{
			get
			{
				if (this.root == null)
					yield break;
				foreach (var file in new FileDataEnumerable(this.root)
					.Where(item => item.IsSelected == true))
					yield return file;
			}
		}

		private int selectedFilesCount = 0;
		int IFileDataProvider.SelectedFilesCount
		{
			get { return this.selectedFilesCount; }
		}

		private EventHandler SelectedFilesCountChangedHandler;
		event EventHandler IFileDataProvider.SelectedFilesCountChanged
		{
			add { SelectedFilesCountChangedHandler += value; }
			remove { SelectedFilesCountChangedHandler -= value; }
		}

		private void raiseSelectedFilesCountChanged()
		{
			if (SelectedFilesCountChangedHandler != null)
				SelectedFilesCountChangedHandler(this, EventArgs.Empty);
		}

		#endregion IFileDataProvider

	}
}

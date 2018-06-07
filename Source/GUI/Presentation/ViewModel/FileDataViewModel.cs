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
			Popup.Show("unpack completed");
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

		private void onFolderRootChanged(FolderData root)
		{
			if (this.root != null)
			{
				foreach (var manager in new SelectableManagerEnumerable(this.root))
				{
					manager.SelectionChanged -= onSelectionChanged;
					manager.Dispose();
				}
			}

			this.root = root;
			NotifyPropertyChanged("RootSource");

			foreach (var manager in new SelectableManagerEnumerable(this.root))
			{
				manager.SelectionChanged += onSelectionChanged;
			}
		}

		private void onSelectionChanged(object sender, EventArgs e)
		{
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
					return Enumerable.Empty<FileData>();
				return new SelectableManagerEnumerable(this.root)
					.Where(item => item.IsAnySelected)
					.SelectMany(item => item.SelectedFiles);
			}
		}

		int IFileDataProvider.SelectedFilesCount
		{
			get
			{
				if (this.root == null)
					return 0;
				return new SelectableManagerEnumerable(this.root)
					.Where(item => item.IsAnySelected)
					.Sum(item => item.SelectedFilesCount);
			}
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

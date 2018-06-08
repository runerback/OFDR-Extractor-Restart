using OFDRExtractor.GUI.Model;
using System;
using System.Linq;

namespace OFDRExtractor.GUI.Business
{
	sealed class SelectableManager
	{
		public SelectableManager(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;

			foreach (var selectable in new SelectableEnumerable(source))
				selectable.IsSelectedChanged += onIsSelectedChanged;
			foreach (var file in new FileDataEnumerable(source))
				file.FileSelectionChanged += onFileSelectionChanged;
		}

		public readonly Model.FolderData source;

		private void onIsSelectedChanged(object sender, EventArgs e)
		{
			FileTreeItem item = (FileTreeItem)sender;
			bool isCurrentSelected = item.IsSelected ?? false;

			if (sender is FolderData)
			{
				//update all inner state
				foreach (var folder in new FolderDataEnumerable((FolderData)sender)
					.Reverse())
				{
					foreach (var file in folder.Files)
						file.SetIsSelected(isCurrentSelected);
					folder.UpdateFolderState(isCurrentSelected);
				}
			}

			//update all upper folder state
			foreach (var folder in new UpperFolderEnumerable(item))
			{
				folder.UpdateFolderState(isCurrentSelected);
			}
		}

		private int selectedFileCount = 0;
		public int SelectedFileCount
		{
			get { return this.selectedFileCount; }
		}

		private void onFileSelectionChanged(object sender, FileSelectionChangedEventArgs e)
		{
			this.selectedFileCount += e.IsSelected ? 1 : -1;

			if (SelectedFileCountChanged != null)
				SelectedFileCountChanged(this, EventArgs.Empty);
		}

		public event EventHandler SelectedFileCountChanged;

		#region Dispose

		private bool disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				var source = this.source;
				foreach (var selectable in new SelectableEnumerable(source))
					selectable.IsSelectedChanged -= onIsSelectedChanged;
				foreach (var file in new FileDataEnumerable(source))
					file.FileSelectionChanged -= onFileSelectionChanged;
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion Dispose

	}
}

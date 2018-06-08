using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	sealed class SelectableHelper
	{
		public SelectableHelper(FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;

			foreach (var item in new SelectableBaseEnumerable(source))
				item.IsSelectedChanged += onIsSelectedChanged;
			foreach (var file in new FileDataEnumerable(source))
				file.FileSelectionChanged += onFileSelectionChanged;
		}

		private readonly FolderData source;
		public FolderData Source
		{
			get { return this.source; }
		}

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
	}
}

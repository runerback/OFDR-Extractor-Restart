using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	//attach on FolderData
	sealed class SelectableManager : ISelectableManager
	{
		public SelectableManager(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			this.source = source;
			foreach (var selectable in source.Selectables)
				selectable.IsSelectedChanged += onIsSelectedChanged;
		}

		private readonly Model.FolderData source;
		
		private void onIsSelectedChanged(object sender, EventArgs e)
		{
			updateStatus();
		}

		private void updateStatus()
		{
			var isAllSelected = true;
			var isAnySelected = false;

			var selectedFiles = this.selectedFiles;
			using (var selectedIterator = this.source.Selectables.GetEnumerator())
			{
				while (selectedIterator.MoveNext())
				{
					var current = selectedIterator.Current;
					if (!current.IsSelected)
					{
						if (isAllSelected)
							isAllSelected = false;

						if (current is Model.FileData)
							selectedFiles.Remove((Model.FileData)current);
					}
					else
					{
						if (!isAnySelected)
							isAnySelected = true;

						if (current is Model.FileData)
							selectedFiles.Add((Model.FileData)current);
					}
				}
			}

			this.IsAllSelected = isAllSelected;
			this.IsAnySelected = isAnySelected;
			this.SelectedFilesCount = selectedFiles.Count;
		}

		private bool isAnySelected;
		public bool IsAnySelected
		{
			get { return this.isAnySelected; }
			private set
			{
				if (this.isAnySelected != value)
				{
					this.isAnySelected = value;
				}
			}
		}

		private bool isAllSelected;
		public bool IsAllSelected
		{
			get { return this.isAllSelected; }
			private set
			{
				if (this.isAllSelected != value)
				{
					this.isAllSelected = value;
				}
			}
		}

		private int selectedFilesCount;
		public int SelectedFilesCount
		{
			get { return this.selectedFilesCount; }
			private set
			{
				if (this.selectedFilesCount != value)
				{
					this.selectedFilesCount = value;
					raiseSelectionChanged();
				}
			}
		}

		private readonly List<Model.FileData> selectedFiles = new List<Model.FileData>();
		public IEnumerable<Model.FileData> SelectedFiles
		{
			get { return this.selectedFiles; }
		}

		public event EventHandler SelectionChanged;
		private void raiseSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		#region Dispose

		private bool disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				foreach (var selectable in source.Selectables)
					selectable.IsSelectedChanged -= onIsSelectedChanged;
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

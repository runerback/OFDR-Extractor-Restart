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

			//check whether source is empty
			if (!new FileDataEnumerable(source).Any())
			{
				this.isAnySelected = true;
				this.isAllSelected = true;
				this.selectedFilesCount = 0;
				this.hasSource = false;
				return;
			}

			this.selectedFiles = new HashSet<Model.FileData>();
			this.source = source;
			foreach (var selectable in new SelectableEnumerable(source, false))
				selectable.IsSelectedChanged += onIsSelectedChanged;
		}

		public readonly Model.FolderData source;

		private bool hasSource = true;
		public bool HasSource
		{
			get { return this.hasSource; }
		}
		
		private void onIsSelectedChanged(object sender, EventArgs e)
		{
			if (sender is Model.FolderData)
			{
				raiseInternalSelectionChanged(SelectableType.Folder);
				if (updateStatus())
				{
					//files under current folder selection changed
					raiseInternalSelectionChanged(SelectableType.File); 
					raiseSelectionChanged();
				}
			}
			else if (sender is Model.FileData)
			{
				if (updateStatus())
				{
					raiseInternalSelectionChanged(SelectableType.File);
					raiseSelectionChanged();
				}
			}
			else
				throw new NotSupportedException("source is not a ISelectable");


		}

		private bool updateStatus()
		{
			var isAllSelected = true;
			var isAnySelected = false;

			var selectedFiles = this.selectedFiles;

			var files = this.source.Files;

			if (!files.Any()) 
				return true;

			using (var fileIterator = files.GetEnumerator())
			{
				bool hasNext = fileIterator.MoveNext();
				if (!hasNext)
					isAnySelected = true;

				while (hasNext)
				{
					var current = fileIterator.Current;
					if (!(current.IsSelected ?? false))
					{
						if (isAllSelected)
							isAllSelected = false;

						selectedFiles.Remove((Model.FileData)current);
					}
					else
					{
						if (!isAnySelected)
							isAnySelected = true;

						selectedFiles.Add((Model.FileData)current);
					}
					hasNext = fileIterator.MoveNext();
				}
			}

			return setIsAllSelected(isAllSelected) |
				setIsAnySelected(isAnySelected) |
				setSelectedFilesCount(selectedFiles.Count);
		}

		#region IsAnySelected

		private bool isAnySelected;
		public bool IsAnySelected
		{
			get { return this.isAnySelected; }
		}

		private bool setIsAnySelected(bool value)
		{
			if (this.isAnySelected != value)
			{
				this.isAnySelected = value;
				return true;
			}
			return false;
		}

		#endregion IsAnySelected

		#region IsAllSelected

		private bool isAllSelected;
		public bool IsAllSelected
		{
			get { return this.isAllSelected; }
		}

		private bool setIsAllSelected(bool value)
		{
			if (this.isAllSelected != value)
			{
				this.isAllSelected = value;
				return true;
			}
			return false;
		}

		#endregion IsAllSelected

		#region SelectedFilesCount

		private int selectedFilesCount;
		public int SelectedFilesCount
		{
			get { return this.selectedFilesCount; }
		}

		private bool setSelectedFilesCount(int value)
		{
			if (this.selectedFilesCount != value)
			{
				this.selectedFilesCount = value;
				return true;
			}
			return false;
		}

		#endregion SelectedFilesCount

		private readonly HashSet<Model.FileData> selectedFiles;
		public IEnumerable<Model.FileData> SelectedFiles
		{
			get { return this.selectedFiles; }
		}

		internal event EventHandler<SelectionChangedEventArgs> InternalSelectionChanged;
		private void raiseInternalSelectionChanged(SelectableType sourceType)
		{
			if (InternalSelectionChanged != null)
				InternalSelectionChanged(this, new SelectionChangedEventArgs(sourceType));
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
				if (this.source != null)
				{
					foreach (var selectable in new SelectableEnumerable(this.source, false))
						selectable.IsSelectedChanged -= onIsSelectedChanged;
				}
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

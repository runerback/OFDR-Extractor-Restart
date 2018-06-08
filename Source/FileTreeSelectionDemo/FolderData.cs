using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	sealed class FolderData : FileTreeItem
	{
		public FolderData(string name)
			: base(name)
		{
		}

		private readonly List<FolderData> folders = new List<FolderData>();
		public IEnumerable<FolderData> Folders
		{
			get { return this.folders; }
		}

		public void Add(FolderData folder)
		{
			folder.Folder = this;
			this.folders.Add(folder);
		}

		private readonly List<FileData> files = new List<FileData>();
		public IEnumerable<FileData> Files
		{
			get { return this.files; }
		}

		public void Add(FileData file)
		{
			file.Folder = this;
			this.files.Add(file);
		}

		public IEnumerable<SelectableBase> Selectables
		{
			get
			{
				return Enumerable.Empty<SelectableBase>()
					.Concat(this.files)
					.Concat(this.folders);
			}
		}

		public bool HasAnyFiles
		{
			get { return new FileDataEnumerable(this).Any(); }
		}

		private int selectedFilesCount = 0;
		public int SelectedFilesCount
		{
			get { return this.selectedFilesCount; }
		}

		public void UpdateFolderState(bool isSourceSelected)
		{
			bool isAllSelected, isAnySelected;
			UpdateSelectionState(isSourceSelected, out isAllSelected, out isAnySelected);

			if (isAllSelected) SetIsSelected(true);
			else if (isAnySelected) SetIsSelected(null);
			else SetIsSelected(false);
		}

		private void UpdateSelectionState(bool isSourceSelected, out bool allSelected, out bool anySelected)
		{
			var allFiles = new FileDataEnumerable(this);
			if (!allFiles.Any())
			{
				allSelected = isSourceSelected;
				anySelected = isSourceSelected;
				return;
			}

			allSelected = true;
			anySelected = false;

			foreach (var isFileSelected in allFiles
				.Select(item => item.IsSelected ?? false))
			{
				if (allSelected && !isFileSelected)
				{
					allSelected = false;
					if (anySelected) break;
				}
				if (!anySelected && isFileSelected)
				{
					anySelected = true;
					if (!allSelected) break;
				}
			}
		}
	}
}

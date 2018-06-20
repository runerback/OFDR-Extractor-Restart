using OFDRExtractor.GUI.Business;
using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	sealed class FolderData : FileTreeItem
	{
		public FolderData(NFSFolder folder, FolderData parentFolder)
			: base(parentFolder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			this.source = folder;

			this.name = folder.Name;
			this.description = folder.ToString();

			this.files = folder.Files
				//.OrderBy(item => item.Extension)
				//.ThenBy(item => item.Filename) //keep origin file order
				.Select(item => new FileData(item, this))
				.ToArray();
			this.subFolders = folder.Folders
				.OrderBy(item => item.Name)
				.Select(item => new FolderData(item, this))
				.ToArray();

			this.hasAnyFiles = new Business.FileDataEnumerable(this).Any();
		}

		private readonly NFSFolder source;
		public NFSFolder Source
		{
			get { return this.source; }
		}

		private string description;
		public string Description
		{
			get { return this.description; }
		}

		private readonly FolderData[] subFolders;
		public IEnumerable<FolderData> SubFolders
		{
			get { return this.subFolders; }
		}

		private readonly FileData[] files;
		public IEnumerable<FileData> Files
		{
			get { return this.files; }
		}

		public IEnumerable<SelectableBase> Selectables
		{
			get
			{
				return Enumerable.Empty<SelectableBase>()
				.Concat(this.files)
				.Concat(this.subFolders);
			}
		}

		private bool hasAnyFiles;
		public bool HasAnyFiles
		{
			get { return this.hasAnyFiles; }
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

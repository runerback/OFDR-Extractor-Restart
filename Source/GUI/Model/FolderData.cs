using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	sealed class FolderData : SelectableBase
	{
		public FolderData(NFSFolder folder, FolderData parentFolder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			this.source = folder;
			this.parentFolder = parentFolder;

			this.name = folder.Name;
			this.description = folder.ToString();

			this.files = folder.Files
				.OrderBy(item => item.Extension)
				.ThenBy(item => item.Filename)
				.Select(item => new FileData(item))
				.ToArray();
			this.subFolders = folder.Folders
				.OrderBy(item => item.Name)
				.Select(item => new FolderData(item, this))
				.ToArray();

			this.hasAnyFiles = new Business.FileDataEnumerable(this).Any();
			if (!this.hasAnyFiles)
				this.IsSelected = true;

			this.selecableManager = new Business.SelectableManager(this);
			this.selecableManager.InternalSelectionChanged += onSelectionChanged;
		}

		private readonly NFSFolder source;
		public NFSFolder Source
		{
			get { return this.source; }
		}

		private string name;
		public string Name
		{
			get { return this.name; }
		}

		private string description;
		public string Description
		{
			get { return this.description; }
		}

		private readonly FolderData parentFolder = null;

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

		public IEnumerable<Business.ISelectable> Selectables
		{
			get { return this.files.Concat<Business.ISelectable>(this.subFolders); }
		}

		private bool hasAnyFiles;
		public bool HasAnyFiles
		{
			get { return this.hasAnyFiles; }
		}

		private readonly Business.SelectableManager selecableManager;
		public ISelectableManager SelectableManager
		{
			get { return this.selecableManager; }
		}
		
		private void onSelectionChanged(object sender, Business.SelectionChangedEventArgs e)
		{
			switch (e.SourceType)
			{
				case Business.SelectableType.File:
						onFileSelectionChanged();
					break;
				case Business.SelectableType.Folder:
						onCurrentFolderSelectionChanged();
					break;
				default: break;
			}
		}

		#region FolderSelectionChanged

		private void onCurrentFolderSelectionChanged()
		{
			bool? isSelected = this.IsSelected;

			if (!isSelected.HasValue) return;

			foreach (var file in this.files)
				file.SetIsSelected(isSelected);
			foreach (var folder in this.subFolders)
				folder.IsSelected = isSelected;
		}

		#endregion FolderSelectionChanged

		#region FileSelectionChanged

		private void onFileSelectionChanged()
		{
			var target = this;
			while (target != null)
			{
				target.updateFolderSelectionState();
				target = target.parentFolder;
			}
		}

		private void updateFolderSelectionState()
		{
			bool allSelected = true;
			bool anySelected = false;

			foreach (var manager in new Business.SelectableManagerEnumerable(this))
			{
				if (allSelected && !manager.IsAllSelected)
				{
					allSelected = false;
					if (anySelected) break;
				}
				if (!anySelected && manager.IsAnySelected)
				{
					anySelected = true;
					if (!allSelected) break;
				}
			}

			if (allSelected)
				SetIsSelected(true);
			else if (anySelected)
				SetIsSelected(null);
			else
				SetIsSelected(false);
		}

		#endregion FileSelectionChanged

		
	}
}

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
			this.selecableManager.SelectionChanged += onSelectionChanged;
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

		private readonly ISelectableManager selecableManager;
		public ISelectableManager SelectableManager
		{
			get { return this.selecableManager; }
		}

		protected override void onIsSelectedChanged()
		{
			bool? isSelected = this.IsSelected;

			if (!isSelected.HasValue) 
				return;

			foreach (var folder in new Business.FolderDataEnumerable(this))
			{
				foreach (var file in folder.files)
				{
					file.ShouldNotifySelectionChanged = false;
					file.IsSelected = isSelected;
					file.ShouldNotifySelectionChanged = true;
				}
				folder.ShouldNotifySelectionChanged = false;
				folder.IsSelected = isSelected;
				folder.ShouldNotifySelectionChanged = true;
			}
		}

		private void onSelectionChanged(object sender, EventArgs e)
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
			this.ShouldNotifySelectionChanged = false;

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
				this.IsSelected = true;
			else if (anySelected)
				this.IsSelected = null;
			else
				this.IsSelected = false;

			this.ShouldNotifySelectionChanged = true;
		}
	}
}

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
		public FolderData(NFSFolder folder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			this.source = folder;

			this.name = folder.Name;
			this.files = folder.Files
				.Select(item => new FileData(item))
				.ToArray();
			this.subFolders = folder.Folders
				.Select(item => new FolderData(item))
				.ToArray();

			this.selecableManager = new Business.SelectableManager(this);
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

		private readonly ISelectableManager selecableManager;
		public ISelectableManager SelectableManager
		{
			get { return this.selecableManager; }
		}

		protected override void onIsSelectedChanged()
		{
			bool isSelected = this.IsSelected;
			foreach (var file in this.files)
			{
				file.ShouldRaiseEvent = false;
				file.IsSelected = isSelected;
				file.ShouldRaiseEvent = true;
			}
		}
	}
}

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
			this.files = new ObservableCollection<FileData>(
				folder.Files.Select(item => new FileData(item)));
			this.subFolders = new ObservableCollection<FolderData>(
				folder.Folders.Select(item => new FolderData(item)));
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

		private readonly AutoInvokeObservableCollection<FolderData> subFolders;
		public IEnumerable<FolderData> SubFolders
		{
			get { return this.subFolders; }
		}
		private readonly AutoInvokeObservableCollection<FileData> files;
		public IEnumerable<FileData> Files
		{
			get { return this.files; }
		}		
	}
}

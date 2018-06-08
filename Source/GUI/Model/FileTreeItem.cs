using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	class FileTreeItem : SelectableBase
	{
		protected FileTreeItem(FolderData parentFolder)
		{
			this.parentFolder = parentFolder;
		}

		protected string name;
		public string Name
		{
			get { return this.name; }
		}

		private FolderData parentFolder;
		public FolderData ParentFolder
		{
			get { return this.parentFolder; }
		}
	}
}

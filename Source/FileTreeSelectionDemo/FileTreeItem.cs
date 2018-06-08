using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	class FileTreeItem : SelectableBase
	{
		protected FileTreeItem(string name)
		{
			this.name = name;
		}

		private string name;
		public string Name
		{
			get { return this.name; }
		}

		public FolderData Folder { get; set; }

		public override string ToString()
		{
			return name;
		}
	}
}

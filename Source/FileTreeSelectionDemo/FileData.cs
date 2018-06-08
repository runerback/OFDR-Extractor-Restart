using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	sealed class FileData : FileTreeItem
	{
		public FileData(string name)
			: base(name)
		{
		}

		public event EventHandler<FileSelectionChangedEventArgs> FileSelectionChanged;

		protected override void onIsSelectedChanged(bool isSelected)
		{
			if (FileSelectionChanged != null)
				FileSelectionChanged(this, new FileSelectionChangedEventArgs(isSelected));
		}
	}
}

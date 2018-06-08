using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	sealed class UpperFolderEnumerable : IEnumerable<FolderData>
	{
		public UpperFolderEnumerable(FileTreeItem source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly FileTreeItem source;

		public IEnumerator<FolderData> GetEnumerator()
		{
			var folder = this.source.Folder;
			while (folder != null)
			{
				yield return folder;
				folder = folder.Folder;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

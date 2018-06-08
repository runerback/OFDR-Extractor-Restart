using OFDRExtractor.GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	class UpperFolderEnumerable : IEnumerable<FolderData>
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
			var folder = this.source.ParentFolder;
			while (folder != null)
			{
				yield return folder;
				folder = folder.ParentFolder;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

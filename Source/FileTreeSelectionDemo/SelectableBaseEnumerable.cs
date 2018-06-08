using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	class SelectableBaseEnumerable : IEnumerable<SelectableBase>
	{
		public SelectableBaseEnumerable(FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly FolderData source;

		public IEnumerator<SelectableBase> GetEnumerator()
		{
			foreach (var folder in new FolderDataEnumerable(this.source))
			{
				foreach (var file in folder.Files)
					yield return file;
				yield return folder;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

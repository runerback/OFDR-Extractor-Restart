using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	class FileDataEnumerable : IEnumerable<FileData>
	{
		public FileDataEnumerable(FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly FolderData source;

		public IEnumerator<FileData> GetEnumerator()
		{
			return new FolderDataEnumerable(this.source)
				.SelectMany(item => item.Files)
				.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

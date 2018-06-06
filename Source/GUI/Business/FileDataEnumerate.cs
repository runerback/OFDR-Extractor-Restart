using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class FileDataEnumerate : IEnumerable<Model.FileData>
	{
		public FileDataEnumerate(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly Model.FolderData source;

		public IEnumerator<Model.FileData> GetEnumerator()
		{
			var source = this.source;

			foreach (var file in source.Files)
				yield return file;

			var folderIteratorStack = new Stack<IEnumerator<Model.FolderData>>();
			folderIteratorStack.Push(source.SubFolders.GetEnumerator());
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}

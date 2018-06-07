using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	class FileDataEnumerable : IEnumerable<Model.FileData>
	{
		public FileDataEnumerable(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly Model.FolderData source;

		public IEnumerator<Model.FileData> GetEnumerator()
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

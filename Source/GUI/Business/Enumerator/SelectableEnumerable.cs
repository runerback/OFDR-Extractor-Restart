using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class SelectableEnumerable : IEnumerable<Model.SelectableBase>
	{
		public SelectableEnumerable(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly Model.FolderData source;

		public IEnumerator<Model.SelectableBase> GetEnumerator()
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

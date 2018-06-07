using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class SelectableManagerEnumerable : IEnumerable<ISelectableManager>
	{
		public SelectableManagerEnumerable(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly Model.FolderData source;

		public IEnumerator<ISelectableManager> GetEnumerator()
		{
			return new FolderDataEnumerable(this.source)
				.Select(item => item.SelectableManager)
				.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

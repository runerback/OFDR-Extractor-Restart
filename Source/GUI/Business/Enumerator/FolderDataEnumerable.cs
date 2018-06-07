using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class FolderDataEnumerable : IEnumerable<Model.FolderData>
	{
		public FolderDataEnumerable(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly Model.FolderData source;

		public IEnumerator<Model.FolderData> GetEnumerator()
		{
			var source = this.source;
			yield return source;

			var folderIteratorStack = new Stack<IEnumerator<Model.FolderData>>();
			folderIteratorStack.Push(source.SubFolders.GetEnumerator());

			while (folderIteratorStack.Count > 0)
			{
				var iterator = folderIteratorStack.Peek();
				if (!iterator.MoveNext())
				{
					folderIteratorStack.Pop();
					iterator.Dispose();
				}
				else
				{
					var current = iterator.Current;
					folderIteratorStack.Push(current.SubFolders.GetEnumerator());

					yield return current;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

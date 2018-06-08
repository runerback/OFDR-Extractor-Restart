using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	sealed class FolderDataEnumerable : IEnumerable<FolderData>
	{
		public FolderDataEnumerable(FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			this.source = source;
		}

		private readonly FolderData source;

		public IEnumerator<FolderData> GetEnumerator()
		{
			var source = this.source;
			yield return source;

			var folderIteratorStack = new Stack<IEnumerator<FolderData>>();
			folderIteratorStack.Push(source.Folders.GetEnumerator());

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
					folderIteratorStack.Push(current.Folders.GetEnumerator());

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

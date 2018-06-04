using OFDRExtractor.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	public sealed class FolderTreeComparer
	{
		public bool AreEqual(Model.NFSFolder nfsRoot, Model.PreparedFolder preparedRoot)
		{
			if (nfsRoot == null)
				throw new ArgumentNullException("nfsRoot");
			if (preparedRoot == null)
				throw new ArgumentNullException("preparedRoot");

			if (!AreNameEqual(nfsRoot, preparedRoot)) return false;
			if (!AreFilesEqual(nfsRoot.Files, preparedRoot.Files)) return false;
			if (!AreFoldersEqual(nfsRoot.Folders, preparedRoot.Folders)) return false;
			return true;
		}

		bool AreFoldersEqual(IEnumerable<NFSFolder> nfs, IEnumerable<PreparedFolder> prepared)
		{
			var leftEmpty = nfs == null || !nfs.Any();
			var rightEmpty = prepared == null || !prepared.Any();

			if (leftEmpty != rightEmpty) return false;
			if (leftEmpty) return true;

			using (var leftIterator = nfs.GetEnumerator())
			using (var rightIterator = prepared.GetEnumerator())
			{
				while (true)
				{
					bool leftEnd = !leftIterator.MoveNext();
					bool rightEnd = !rightIterator.MoveNext();
					if (leftEnd != rightEnd) return false;
					if (leftEnd) return true;

					var left = leftIterator.Current;
					var right = rightIterator.Current;
					if (!AreNameEqual(left, right)) return false;
					if (!AreFilesEqual(left.Files, right.Files)) return false;
					if (!AreFoldersEqual(left.Folders, right.Folders)) return false;
				}
			}
		}

		bool AreFilesEqual(IEnumerable<IName> nfs, IEnumerable<IName> prepared)
		{
			var leftEmpty = nfs == null || !nfs.Any();
			var rightEmpty = prepared == null || !prepared.Any();

			if (leftEmpty != rightEmpty) return false;
			if (leftEmpty) return true;

			using(var leftIterator = nfs.GetEnumerator())
			using (var rightIterator = prepared.GetEnumerator())
			{
				while (true)
				{
					bool leftEnd = !leftIterator.MoveNext();
					bool rightEnd = !rightIterator.MoveNext();
					if (leftEnd != rightEnd) return false;
					if (leftEnd) return true;

					if (!AreNameEqual(leftIterator.Current, rightIterator.Current)) return false;
				}
			}
		}

		bool AreNameEqual(IName left, IName right)
		{
			return left == null ? right == null :
				string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
		}
	}
}

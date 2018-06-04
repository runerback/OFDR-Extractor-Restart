using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	public sealed class PreparedFolderBranchRefMap
	{
		public PreparedFolderBranchRefMap(IEnumerable<PreparedFolderBranch> branches)
		{
			if (branches == null || !branches.Any())
				throw new ArgumentNullException("branches");
			this.refCountMap = branches
				.SelectMany(item => item.Nodes)
				.GroupBy(item => item, new PreparedFolderBranchNodeComparer())
				.ToDictionary(item => item.Key, item => item.Count());

			var AIRefCount = getRefCount("ai");
		}

		private readonly Dictionary<PreparedFolderBranchNode, int> refCountMap =
			new Dictionary<PreparedFolderBranchNode, int>(new PreparedFolderBranchNodeComparer());
		private readonly object mapLock = new object();

		public bool HasReference(PreparedFolderBranchNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			lock(mapLock)
				return this.refCountMap.ContainsKey(node);
		}

		public int DecreaseReferenceCount(PreparedFolderBranchNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			lock (mapLock)
			{
				int count;
				if (this.refCountMap.TryGetValue(node, out count))
				{
					if (--count == 0)
						this.refCountMap.Remove(node);
				}
				return count;
			}
		}

		private int getRefCount(string name)
		{
			return this.refCountMap.Keys
				.Count(item => item.Name == name);
		}
	}
}

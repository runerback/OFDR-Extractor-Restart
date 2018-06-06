using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	sealed class PreparedFolderBranchRefMap
	{
		public PreparedFolderBranchRefMap(IEnumerable<PreparedFolderBranch> branches)
		{
			if (branches == null || !branches.Any())
				throw new ArgumentNullException("branches");

			var comparer = new PreparedFolderBranchNodeComparer();
			this.refCountMap = branches
				.SelectMany(item => item.Nodes)
				.GroupBy(item => item, comparer)
				.ToDictionary(
					item => item.Key, 
					item => item.Count(),
					comparer);
		}

		private readonly Dictionary<PreparedFolderBranchNode, int> refCountMap;
		private readonly object mapLock = new object();

		public int Count
		{
			get
			{
				lock (mapLock)
				{
					return this.refCountMap.Count;
				}
			}
		}

		public bool HasReference(PreparedFolderBranchNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			lock (mapLock)
				return this.refCountMap.ContainsKey(node);
		}

		public int DecreaseReferenceCount(PreparedFolderBranchNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			lock (mapLock)
			{
				int count;
				var map = this.refCountMap;
				if (map.TryGetValue(node, out count))
				{
					if (--count == 0)
						map.Remove(node);
					else
						map[node] = count;
				}
				return count;
			}
		}

		private int getRefCount(string name)
		{
			lock (mapLock)
			{
				int refCount = 0;
				var nodes = this.refCountMap.Keys
					.Where(item => item.Name == name);
				foreach (var node in nodes)
				{
					int value;
					if (this.refCountMap.TryGetValue(node, out value))
						refCount += value;
				}
				return refCount;
			}
		}
	}
}

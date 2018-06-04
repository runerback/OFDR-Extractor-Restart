using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Model
{
	public sealed class PreparedFolderBranch
	{
		public PreparedFolderBranch(IEnumerable<string> orderedNodes)
		{
			if (orderedNodes == null || !orderedNodes.Any())
				throw new ArgumentNullException("orderedNodes");

			var nodes = orderedNodes.ToArray();

			this.fullPath = string.Join("/", nodes);
			this.nodes = nodes
				.Select((name, index) => new PreparedFolderBranchNode(nodes.Take(index + 1), name))
				.ToArray();
		}

		private string fullPath;
		public string FullPath
		{
			get { return this.fullPath; }
		}

		private IEnumerable<PreparedFolderBranchNode> nodes;
		public IEnumerable<PreparedFolderBranchNode> Nodes
		{
			get { return this.nodes; }
		}

		public override string ToString()
		{
			return this.fullPath;
		}
	}
}

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

			this.fullPath = string.Join(NODE_SPLITER.ToString(), nodes);
			this.nodes = nodes
				.Select((name, index) => new PreparedFolderBranchNode(nodes.Take(index + 1), name))
				.ToArray();
		}

		public PreparedFolderBranch(string branch)
		{
			if (string.IsNullOrWhiteSpace(branch))
				throw new ArgumentNullException("branch");
			var nodes = branch.Split(NODE_SPLITER); //do not remove empty item here as a validation
			if (nodes.Length == 0)
				throw new ArgumentException("branch. wrong format");

			this.fullPath = branch;

			var branchNodes = nodes
				.Select((name, index) => new PreparedFolderBranchNode(nodes.Take(index + 1), name))
				.ToArray();
			this.nodes = branchNodes;
			this.nodeCount = branchNodes.Length;
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

		private int nodeCount;
		public int NodeCount
		{
			get { return this.nodeCount; }
		}

		public const char NODE_SPLITER = '/';

		public bool IsStartsWith(string branch)
		{
			if (string.IsNullOrEmpty(branch))
				return false; //starts with nothing

			var source = this.fullPath;
			var target = branch;
			if (source.Length < target.Length)
				return false; //target should be shorter than source

			using (var targetIterator = target.GetEnumerator())
			using (var sourceIterator = source.GetEnumerator())
			{
				while (targetIterator.MoveNext() && sourceIterator.MoveNext())
				{
					if (targetIterator.Current != sourceIterator.Current)
						return false;
				}

				if (!sourceIterator.MoveNext())
					return true; //exactly same

				if (sourceIterator.Current == PreparedFolderBranch.NODE_SPLITER)
					return true; //starts with
			}
			return false; //not end with NODE_SPLITER means not start with Nodes
		}

		public override string ToString()
		{
			return this.fullPath;
		}
	}
}

using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	public sealed class NFSTreeBuilder
	{
		public NFSTreeBuilder(NFSFolder nfsRoot, PreparedFolder preparedRoot)
		{
			if (nfsRoot == null)
				throw new ArgumentNullException("nfsRoot");
			if (preparedRoot == null)
				throw new ArgumentNullException("preparedRoot");

			this.NFSRoot = nfsRoot.Copy();
			this.PreparedRoot = preparedRoot.Copy();
		}

		private readonly NFSFolder NFSRoot;
		private readonly PreparedFolder PreparedRoot;

		public bool TryBuild(out NFSFolder root)
		{
			root = null;

			var preparedBranches = new PreparedFolderBranches(this.PreparedRoot);
			var nfsFolders = this.NFSRoot.Folders.ToArray();
			foreach (var branch in preparedBranches.Branches)
			{
				var nodes = branch.Nodes;
				//match whole branch
				using (var nodeIterator = nodes.GetEnumerator())
				{
					NFSFolder nfsNode = null;
					while (nodeIterator.MoveNext())
					{
						throw new NotImplementedException();
					}
				}
			}

			return false;
		}

		[Obsolete("not work")]
		private IDictionary<string, NFSFolder> buildNFSFolderBranchMap(NFSFolder root)
		{
			if (!root.Folders.Any())
				throw new ArgumentException("sub folders empty");

			var map = new Dictionary<string, NFSFolder>();
			int depth = 5;
			var branch = new NFSFolderBranch(depth);

			foreach (var folder in root.Folders)
			{
				if (folder.Folders.Any())
					throw new InvalidOperationException("the root here should be only one layer");
				branch.Add(folder.Name);
				string branchPath = branch.Path;
				try
				{
					map.Add(branchPath, folder);
				}
				catch(ArgumentException)
				{
					throw new Exception(string.Format("branch path [{0}] already exists", branchPath));
				}
			}

			return map;
		}

		[Obsolete("not work")]
		class NFSFolderBranch
		{
			public NFSFolderBranch(int depth)
			{
				if (depth <= 0)
					throw new ArgumentOutOfRangeException("depth. should greater than 1");

				this.depth = depth;
				this.nodes = new string[depth];
			}

			private readonly int depth;
			private readonly string[] nodes;
			private int index = 0;
			private readonly string spliter = "/";

			public void Add(string node)
			{
				if (string.IsNullOrEmpty(node))
					throw new ArgumentNullException("node");

				var nodes = this.nodes;
				var index = this.index;
				int bound = depth - 1;
				if (index <= bound)
				{
					nodes[index++] = node;
					this.index = index;
					this.path = string.Join(spliter, nodes.Take(index));
				}
				else
				{
					for (int i = 0; i < bound; i++)
					{
						nodes[i] = nodes[i + 1];
					}
					nodes[bound] = node;
					this.path = string.Join(spliter, nodes);
				}
			}

			private string path;
			public string Path
			{
				get { return this.path; }
			}
		}
	}
}

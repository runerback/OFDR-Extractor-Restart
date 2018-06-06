using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	public sealed class NFSTreeBuilder
	{
		public NFSTreeBuilder(NFSFolder nfsRoot, PreparedFolderBranchesManager branchesManager)
		{
			if (nfsRoot == null)
				throw new ArgumentNullException("nfsRoot");
			if (branchesManager == null)
				throw new ArgumentNullException("branchesManager");

			this.nfsRoot = nfsRoot.Copy();
			this.branchesManager = branchesManager;
		}

		private readonly NFSFolder nfsRoot;
		private readonly PreparedFolderBranchesManager branchesManager;
		private readonly object buildLock = new object();

		private NFSFolder GetRoot()
		{
			return this.nfsRoot.Copy();
		}

		public NFSFolder Build()
		{
			lock (buildLock)
			{
				var root = NFSFolder.CreateRoot();
				var nfsFolders = this.GetRoot().Folders.ToList();

				var branchesManager = this.branchesManager;
				var branches = branchesManager.Branches;
				var nodeRefCountMap = branchesManager.CreateRefMap();

				foreach (var branch in branches)
				{
					var nodes = branch.Nodes;
					int nodesCount = branch.NodeCount;

					var matchedStack = new Stack<NFSFolderNodePair>(nodesCount);
					foreach (var node in nodes)
					{
						var folder = nfsFolders
							.FirstOrDefault(item => item.Name == node.Name);
						if (folder == null) break;

						matchedStack.Push(new NFSFolderNodePair(folder, node));
					}

					if (matchedStack.Count != nodesCount)
						throw new Exception(string.Format("[{0}] not match to [{1}]",
							string.Join(", ", matchedStack.Select(item => item.Folder.Name)),
							branch.FullPath));

					//add inner folder to outer parent folder
					NFSFolder subFolder = null;
					while (matchedStack.Count > 0)
					{
						var pair = matchedStack.Pop();

						var folder = pair.Folder;
						var node = pair.Node;

						int refCount = nodeRefCountMap.DecreaseReferenceCount(node);
						if (refCount == 0)
							nfsFolders.Remove(folder);

						if (subFolder != null)
							folder.Add(subFolder);

						if (matchedStack.Count == 0)
						{
							root.Add(folder);
							break;
						}

						subFolder = folder;
					}
				}

				if (nfsFolders.Count != 0)
					throw new Exception(
						string.Format("Unassigned folders: {0}", string.Join(", ", nfsFolders.Select(item => item.Name))));

				return root;
			}
		}

		class NFSFolderNodePair
		{
			public NFSFolderNodePair(NFSFolder folder, PreparedFolderBranchNode node)
			{
				if (folder == null)
					throw new ArgumentNullException("folder");
				if (node == null)
					throw new ArgumentNullException("node");
				if (folder.Name != node.Name)
					throw new ArgumentException("folder and node are mismatched");

				this.folder = folder;
				this.node = node;
			}

			private readonly NFSFolder folder;
			public NFSFolder Folder
			{
				get { return this.folder; }
			}

			private readonly PreparedFolderBranchNode node;
			public PreparedFolderBranchNode Node
			{
				get { return this.node; }
			}

			public override string ToString()
			{
				return this.node.Name;
			}
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

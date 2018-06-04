using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFDRExtractor.Model;
using OFDRExtractor.Business;

namespace OFDRExtractor.UnitTest
{
	[TestClass]
	public class TwoRootTest
	{
		private readonly NFSRootTest nfs = new NFSRootTest();
		private readonly PreparedRootTest prepared = new PreparedRootTest();

		[TestInitialize]
		public void LoadTwoRoot()
		{
			nfs.ReadNFSRoot();
			prepared.ReadPreparedRoot();
		}

		[TestMethod]
		public void CompareTwoTree()
		{
			var nfsRoot = this.nfs.Root;
			var preparedRoot = this.prepared.Root;

			Assert.IsFalse(new FolderTreeComparer()
				.AreEqual(nfsRoot, preparedRoot));
		}

		[TestMethod]
		public void CheckNFSFolder()
		{
			var nfsRoot = this.nfs.Root;
			var preparedRoot = this.prepared.Root;

			var preparedBranches = new PreparedFolderBranches(preparedRoot);
			var nodeRefCountMap = preparedBranches.CreateRefMap();

			var nfsFolders = nfsRoot.Folders.ToList();
			foreach (var branch in preparedBranches.Branches)
			{
				var nodes = branch.Nodes;
				int nodesCount = nodes.Count();

				var matchedStack = new Stack<NFSFolderNodePair>(nodesCount);
				foreach (var node in nodes)
				{
					var folder = nfsFolders
						.FirstOrDefault(item => item.Name == node.Name);
					if (folder == null) break;

					matchedStack.Push(new NFSFolderNodePair(folder, node));
				}
				if (matchedStack.Count != nodesCount)
				{
					Assert.Fail(string.Format("[{0}] not match to [{1}]",
						string.Join(", ", matchedStack.Select(item => item.Folder.Name)),
						branch.FullPath));
				}
				else
				{
					NFSFolder subFolder = null;
					while (matchedStack.Count > 0)
					{
						var pair = matchedStack.Pop();

						var folder = pair.Folder;
						var node = pair.Node;
						if (nodeRefCountMap.DecreaseReferenceCount(node) == 0)
						{
							nfsFolders.Remove(folder);
						}

						if (subFolder != null)
						{
							folder.Add(subFolder);
						}
						subFolder = folder;
					}
				}
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

		[TestMethod]
		public void BuildNFSTree()
		{
			var preparedRoot = this.prepared.Root;
			var builder = new NFSTreeBuilder(this.nfs.Root, preparedRoot);
			NFSFolder result;
			Assert.IsTrue(builder.TryBuild(out result));
			Assert.IsNotNull(result);
			Assert.IsTrue(new Business.FolderTreeComparer()
				.AreEqual(result, preparedRoot));
		}
	}
}

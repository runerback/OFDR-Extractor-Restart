using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFDRExtractor.Model;
using OFDRExtractor.Business;
using System.IO;

namespace OFDRExtractor.UnitTest
{
	[TestClass]
	public class TwoRootTest
	{
		private readonly string branchData = @"branches.txt";

		private NFSRootTest nfs = new NFSRootTest();
		private PreparedFolderBranches branchesContainer;

		[TestInitialize]
		public void InitializeTwoRootTest()
		{
			nfs.ReadNFSRoot();
			Assert.IsTrue(File.Exists(branchData), "branches data missing");
			var lines = File.ReadAllLines(branchData);
			Assert.IsTrue(lines != null && lines.Length > 0, "empty branches");
			this.branchesContainer = new PreparedFolderBranches(lines);
		}

		[TestMethod]
		public void CheckNFSFolder()
		{
			var nfsRoot = this.nfs.Root;

			var branchesContainer = this.branchesContainer;
			var branches = branchesContainer.Branches;
			var nodeRefCountMap = branchesContainer.CreateRefMap();

			var nfsFolders = nfsRoot.Folders.ToList();

			int startCount = nfsFolders.Count;
			int totalNodesCount = nodeRefCountMap.Count;

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

						int refCount = nodeRefCountMap.DecreaseReferenceCount(node);
						if (refCount  == 0)
							nfsFolders.Remove(folder);

						if (subFolder != null)
						{
							folder.Add(subFolder);
						}
						if (matchedStack.Count == 0) break;
						subFolder = folder;
					}
				}
			}

			Assert.AreEqual(totalNodesCount, startCount - nfsFolders.Count);
			Assert.AreEqual(0, nfsFolders.Count,
				string.Format("still some remained: {0}", string.Join(", ", nfsFolders.Select(item => item.Name))));
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
	}
}

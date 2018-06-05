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
		private PreparedFolderBranchesManager branchesManager;

		[TestInitialize]
		public void InitializeTwoRootTest()
		{
			nfs.ReadNFSRoot();
			Assert.IsTrue(File.Exists(branchData), "branches data missing");
			var lines = File.ReadAllLines(branchData);
			Assert.IsTrue(lines != null && lines.Length > 0, "empty branches");
			this.branchesManager = new PreparedFolderBranchesManager(lines);
		}

		[TestMethod]
		public void BuildNFSRoot()
		{
			var builder = new NFSTreeBuilder(this.nfs.Root, this.branchesManager);
			var nfsFolderRoot = builder.Build();

			//silly way to get PreparedFolderRoot
			new GenerateFoldersFromBranches().GenerateDataWinFolderTree();
			var preparedFolderTest = new PreparedRootTest();
			preparedFolderTest.ReadPreparedRoot();
			var preparedFolderRoot = preparedFolderTest.Root;

			bool areEquals = new FolderTreeComparer().AreEqual(
				nfsFolderRoot, 
				preparedFolderRoot, 
				FolderTreeCompareLevel.Folder);
			Assert.IsTrue(areEquals, "two root are not same");
		}
	}
}

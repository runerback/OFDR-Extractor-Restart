﻿using System;
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
		private NFSFolderBranchesManager branchesManager;

		[TestInitialize]
		public void InitializeTwoRootTest()
		{
			nfs.ReadNFSRoot();
			Assert.IsTrue(File.Exists(branchData), "branches data missing");
			var lines = File.ReadAllLines(branchData);
			Assert.IsTrue(lines != null && lines.Length > 0, "empty branches");
			this.branchesManager = new NFSFolderBranchesManager(
				lines,
				new ProgressReporterInConsole());
		}

		[TestMethod]
		public void BuildNFSRoot()
		{
			var builder = new NFSTreeBuilder(this.nfs.Root, this.branchesManager);
			var nfsFolderRoot = builder.Build(new ProgressReporterInConsole());

			//silly way to get PreparedFolderRoot
			new GenerateFoldersFromBranches().GenerateDataWinFolderTree();
			//var preparedFolderTest = new PreparedRootTest();
			//preparedFolderTest.ReadPreparedRoot();
			//var preparedFolderRoot = preparedFolderTest.Root;

			//bool areEquals = new FolderTreeComparer().AreEqual(
			//	nfsFolderRoot, 
			//	preparedFolderRoot, 
			//	FolderTreeCompareLevel.Folder);
			//Assert.IsTrue(areEquals, "two root are not same");
		}

		[TestMethod]
		public void TravelTest()
		{
			var nfsRoot = this.nfs.Root;
			int count = nfsRoot.Folders.Count() + 1;

			Assert.AreEqual(count, travelFolder(nfsRoot), "flatten");

			var builder = new NFSTreeBuilder(this.nfs.Root, this.branchesManager);
			var builtNFSRoot = builder.Build(new ProgressReporterInConsole());

			Assert.AreEqual(count, travelFolder(builtNFSRoot), "built");
		}

		private int travelFolder(NFSFolder source)
		{
			int count = 0;

			count++;

			var iteratorStack = new Stack<IEnumerator<NFSFolder>>();
			iteratorStack.Push(source.Folders.GetEnumerator());

			while (iteratorStack.Count > 0)
			{
				var iterator = iteratorStack.Peek();
				if (!iterator.MoveNext())
				{
					iteratorStack.Pop();
				}
				else
				{
					var current = iterator.Current;
					iteratorStack.Push(current.Folders.GetEnumerator());

					count++;
				}
			}

			return count;
		}
	}
}

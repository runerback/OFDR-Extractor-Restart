using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	static class FileTreeBuilder
	{
		public static FolderData Root
		{
			get { return buildSimpleTree(); }	
		}

		static FolderData buildSimpleTree()
		{
			var root = new FolderData("data_win");

			var subFo1 = new FolderData("fo_1");
			subFo1.Add(new FileData("fi_1_1"));
			subFo1.Add(new FileData("fi_1_2"));

			var subFo2 = new FolderData("fo_2");

			var subFo3 = new FolderData("fo_3");
			subFo3.Add(new FileData("fi_3_1"));

			root.Add(subFo1);
			root.Add(subFo2);
			root.Add(subFo3);

			return root;
		}

		public static FolderData NFSRoot
		{
			get { return buildFromExternalData(); }
		}

		static FolderData buildFromExternalData()
		{
			string linesFile = "lines.txt";
			if (!File.Exists(linesFile))
				throw new FileNotFoundException(linesFile);
			var lines = File.ReadAllLines(linesFile);
			var nfsRootFlatten = OFDRExtractor.Model.NFSFolder.Load(lines, null).Result;

			string branchesFile = "branches.txt";
			if (!File.Exists(branchesFile))
				throw new FileNotFoundException(branchesFile);
			var branches = File.ReadAllLines(branchesFile);
			var branchesManager = 
				new OFDRExtractor.Business.NFSFolderBranchesManager(branches, null);

			var nfsRoot = new OFDRExtractor.Business.NFSTreeBuilder(nfsRootFlatten, branchesManager)
				.Build(null);

			var root = new FolderData(nfsRoot.Name);

			foreach (var file in nfsRoot.Files)
				root.Add(new FileData(file.Name));

			var folderDataStack = new Stack<FolderData>();
			folderDataStack.Push(root);

			var nfsFolderIteratorStack = new Stack<IEnumerator<OFDRExtractor.Model.NFSFolder>>();
			nfsFolderIteratorStack.Push(nfsRoot.Folders.GetEnumerator());

			while (nfsFolderIteratorStack.Count > 0)
			{
				var iterator = nfsFolderIteratorStack.Peek();
				if (!iterator.MoveNext())
				{
					nfsFolderIteratorStack.Pop();
					iterator.Dispose();

					if (folderDataStack.Count > 0)
						folderDataStack.Pop();
				}
				else
				{
					var current = iterator.Current;
					nfsFolderIteratorStack.Push(current.Folders.GetEnumerator());

					var folderData = folderDataStack.Peek();

					var nextFolderData = new FolderData(current.Name);
					foreach (var nfsFile in current.Files)
						nextFolderData.Add(new FileData(nfsFile.Name));

					folderData.Add(nextFolderData);

					folderDataStack.Push(nextFolderData);
				}
			}

			return root;
		}
	}
}

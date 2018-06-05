using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OFDRExtractor.UnitTest
{
	[TestClass]
	public class GenerateFoldersFromBranches
	{
		private readonly string targetRoot = @"../../data_win";
		private readonly string branchData = @"branches.txt";

		[TestMethod]
		public void GenerateDataWinFolderTree()
		{
			var root = Path.GetFullPath(targetRoot);			
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}
			else
			{
				revertFolders(root);
			}

			var branches = File.ReadAllLines(branchData);
			try
			{
				foreach (var branch in branches)
				{
					var path = Path.Combine(root, branch);
					Console.WriteLine("creating {0}", branch);
					Directory.CreateDirectory(path);
				}
			}
			catch
			{
				revertFolders(root);
				throw;
			}
		}

		private void revertFolders(string root)
		{
			foreach (var layer1 in Directory.GetDirectories(root))
				Directory.Delete(layer1, true);
		}
	}
}

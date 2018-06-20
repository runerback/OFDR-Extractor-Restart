using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using OFDRExtractor.Model;

namespace OFDRExtractor.UnitTest
{
	[TestClass]
	public class NFSRootTest
	{
		private Model.NFSFolder nfsRoot = null;
		public Model.NFSFolder Root
		{
			get { return this.nfsRoot; }
		}

		[TestInitialize]
		public void ReadNFSRoot()
		{
			var lines = File.ReadAllLines("lines.txt");
			var root = Model.NFSFolder
				.Load(lines, new ProgressReporterInConsole(false))
				.Result;
			Assert.IsNotNull(root);
			Assert.IsTrue(root.Folders.Any());
			this.nfsRoot = root;
		}

		[TestMethod]
		//same name, size and folder, repeat 2 times
		public void RepeatFileTest()
		{
			var root = this.nfsRoot;
			var audio = root.Folders
				.First(item => item.Name.Equals("audio", StringComparison.OrdinalIgnoreCase));
			var amb_day = audio.Files
				.First(item => item.Name.Equals("amb_day.fsb", StringComparison.OrdinalIgnoreCase));
			Assert.AreEqual(1, amb_day.Order);
		}

		[TestMethod]
		//same name and size but different folder
		public void RepeatFileTest2()
		{
			var root = this.nfsRoot;

			string filename = "freelook.xml";

			var actionmap = root.Folders
				.First(item => item.Name.Equals("actionmap", StringComparison.OrdinalIgnoreCase));
			var freelook1 = actionmap.Files
				.First(item => item.Name.Equals(filename, StringComparison.OrdinalIgnoreCase));

			var legacy = root.Folders
				.First(item => item.Name.Equals("legacy", StringComparison.OrdinalIgnoreCase));
			var freelook2 = legacy.Files
				.First(item => item.Name.Equals(filename, StringComparison.OrdinalIgnoreCase));

			Assert.AreEqual(freelook1.Order, freelook2.Order);
		}

		private static readonly string filename = "nsf_root.xml";

		[TestMethod]
		public void SaveNFSRoot()
		{
			var root = this.nfsRoot;
			using (var output = new FileStream(filename, FileMode.Create))
			using (var stream = new Business.FolderSerializer().Serialize(root))
			{
				stream.WriteTo(output);
			}
		}

		[TestMethod]
		public void CopyNFSFolder()
		{
			var root = NFSFolder.CreateRoot();

			var folder1 = new NFSFolder(1, "Folder 1");
			var folder2 = new NFSFolder(2, "Folder 2");

			root.Add(folder1);
			root.Add(folder2);

			var copyRoot = root.Copy();

			Assert.AreNotSame(folder1, copyRoot.Folders.First());
			Assert.AreNotSame(folder2, copyRoot.Folders.ElementAt(1));

			folder1.Add(folder2);

			Assert.AreSame(root.Folders.First().Folders.First(), folder2);
			Assert.IsFalse(copyRoot.Folders.First().Folders.Any());
		}
	}
}

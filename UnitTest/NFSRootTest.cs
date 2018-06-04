using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;

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
			var root = Model.NFSFolder.Load(lines);
			Assert.IsNotNull(root);
			Assert.IsTrue(root.Folders.Any());
			this.nfsRoot = root;

			//foreach (var folder in root.Folders)
			//	Console.WriteLine(folder.Name);
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
	}
}

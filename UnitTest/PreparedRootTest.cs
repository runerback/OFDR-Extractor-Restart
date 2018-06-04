using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace OFDRExtractor.UnitTest
{
	[TestClass]
	public class PreparedRootTest
	{
		private Model.PreparedFolder preparedRoot;
		public Model.PreparedFolder Root
		{
			get { return this.preparedRoot; }
		}

		[TestInitialize]
		public void ReadPreparedRoot()
		{
			var root = Model.PreparedFolder.Load("../../data_win");
			Assert.IsNotNull(root);
			Assert.IsTrue(root.Folders.Any());
			this.preparedRoot = root;
		}

		private static readonly string filename = "prepared_root.xml";

		[TestMethod]
		public void SavePreparedRoot()
		{
			var root = this.preparedRoot;
			using (var output = new FileStream(filename, FileMode.Create))
			using (var stream = new Business.FolderSerializer().Serialize(root))
			{
				stream.WriteTo(output);
			}
		}
	}
}

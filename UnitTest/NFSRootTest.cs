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
		[TestMethod]
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
		public void ListAllFileExtension()
		{
			var root = this.nfsRoot;
			HashSet<string> set = new HashSet<string>(EqualityComparer<string>.Default);
			foreach (var ext in root.Folders.SelectMany(folder => folder.Files.Select(file => file.Extension.ToLower())))
				set.Add(ext);
			foreach (var ext in set.OrderBy(item => item))
				Console.WriteLine("." + ext);
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

		[TestMethod]
		//list all repeat files
		public void RepeatFileTest3()
		{
			var root = this.nfsRoot;
			foreach (var group in root.Folders
				.SelectMany(folder => folder.Files
					.Select(file => new RepeatFileData(file.Name, file.Size, folder.Name)))
				.GroupBy(item => item, RepeatFileData.Comparer))
			{
				using (var iterator = group.GetEnumerator())
				{
					if (!iterator.MoveNext() || !iterator.MoveNext()) continue;
				}

				Console.WriteLine();
				foreach (var data in group)
					Console.WriteLine(data);
			}
			Console.WriteLine();
		}

		sealed class RepeatFileData
		{
			public RepeatFileData(string filename, long filesize, string foldername)
			{
				this.fileName = filename;
				this.fileSize = filesize;
				this.folderName = foldername;

				this.hashcode = filename.ToLower().GetHashCode() ^ filesize.GetHashCode();
			}

			private string fileName;
			public string FileName
			{
				get { return this.fileName; }
			}

			private long fileSize;
			public long FileSize
			{
				get { return this.fileSize; }
			}

			private string folderName;
			public string FolderName
			{
				get { return this.folderName; }
			}

			private readonly int hashcode;

			public override string ToString()
			{
				return string.Format("{0} - {1} {2}", 
					folderName, 
					fileName, 
					fileSize);
			}

			public static readonly IEqualityComparer<RepeatFileData> Comparer = new RepeatFileDataComparer();

			sealed class RepeatFileDataComparer : IEqualityComparer<RepeatFileData>
			{
				bool IEqualityComparer<RepeatFileData>.Equals(RepeatFileData x, RepeatFileData y)
				{
					return x == null ? y == null : x.hashcode == y.hashcode;
				}

				int IEqualityComparer<RepeatFileData>.GetHashCode(RepeatFileData obj)
				{
					if (obj == null) return 0;
					return obj.hashcode;
				}
			}
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

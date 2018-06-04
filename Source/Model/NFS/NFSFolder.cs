using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OFDRExtractor.Model
{
	public sealed class NFSFolder : NFSLine
	{
		private NFSFolder() { }

		public NFSFolder(int index, string name)
			: base(index, 0, name, -1)
		{
		}

		private List<NFSFile> files = new List<NFSFile>();
		public IEnumerable<NFSFile> Files
		{
			get { return this.files; }
		}

		public void Add(NFSFile file)
		{
			this.files.Add(file);
		}

		private List<NFSFolder> folders = new List<NFSFolder>();
		public IEnumerable<NFSFolder> Folders
		{
			get { return this.folders; }
		}

		public void Add(NFSFolder folder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			if (this.folders.Contains(folder)) return;

			this.folders.Add(folder);
		}

		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("folder");

			writer.WriteStartAttribute("name");
			writer.WriteValue(this.Name);
			writer.WriteEndAttribute();

			foreach (var folder in this.folders)
				folder.WriteXml(writer);
			foreach (var file in this.files)
				file.WriteXml(writer);

			writer.WriteEndElement();
		}

		public override string ToString()
		{
			return string.Format("{0}: {1} files, {2} folders",
				this.Name,
				this.files.Count,
				this.folders.Count);
		}

		public static NFSFolder CreateRoot()
		{
			return new NFSFolder(-1, "data_win");
		}

		public NFSFolder Copy()
		{
			var folder = new NFSFolder(this.Index, this.Name);

			folder.files.AddRange(this.files.Select(file =>
				new NFSFile(file.Filename, file.Extension, folder, file.Index, file.Order, file.Name, file.Size)));
			folder.folders.AddRange(this.folders.Select(item => item.Copy()));

			return folder;
		}

		public static NFSFolder Load(IEnumerable<string> nfsLines)
		{
			var root = NFSFolder.CreateRoot();
			if (nfsLines == null || !nfsLines.Any())
				return root;

			foreach (var folder in new NFSFoldersReader(nfsLines).Read())
				root.Add(folder);
			return root;
		}

		class NFSFoldersReader
		{
			public NFSFoldersReader(IEnumerable<string> nfsLines)
			{
				this.nfsLines = nfsLines;
			}

			private readonly IEnumerable<string> nfsLines;
			private readonly object readLock = new object();
			private readonly Dictionary<string, int> fileOrderMap = new Dictionary<string, int>();

			private readonly Regex folderRegex = new Regex(
					@"^(?<folder>[^.:]+)\s+0$",
					RegexOptions.Compiled);
			private readonly Regex fileRegex = new Regex(
					@"^(?<file>(?<filename>.*?)\.(?<extension>.*?))\s+(?<size>\d+)$",
					RegexOptions.Compiled);

			public NFSFolder[] Read()
			{
				lock (readLock)
				{
					this.fileOrderMap.Clear();
					return readLines().ToArray();
				}
			}

			private IEnumerable<NFSFolder> readLines()
			{
				var folderRegex = this.folderRegex;
				var fileRegex = this.fileRegex;

				int folderIndex = 0;
				int fileIndex = 0;

				NFSFolder currentFolder = null;
				string previousFolderName = null;
				foreach (var line in nfsLines)
				{
					var folderMatch = folderRegex.Match(line);
					if (folderMatch.Success)
					{
						string folderName = folderMatch.Groups["folder"].Value;

						if (string.Equals(previousFolderName, folderName, StringComparison.OrdinalIgnoreCase))
							continue;
						previousFolderName = folderName;

						currentFolder = new NFSFolder(folderIndex++, folderName);
						yield return currentFolder;

						fileIndex = 0;

						continue;
					}

					var fileMatch = fileRegex.Match(line);
					if (fileMatch.Success)
					{
						var groups = fileMatch.Groups;

						string name = groups["file"].Value;
						string filename = groups["filename"].Value;
						string extension = groups["extension"].Value;
						long size;
						long.TryParse(groups["size"].Value, out size);
						int order = getFileOrder(name);

						var file = new NFSFile(filename, extension, currentFolder, fileIndex++, order, name, size);
						currentFolder.Add(file);
						continue;
					}
				}
			}

			private int getFileOrder(string name)
			{
				if (string.IsNullOrEmpty(name)) return 0;

				var map = this.fileOrderMap;
				int currentCount;
				if (map.TryGetValue(name, out currentCount))
				{
					currentCount++;

					map[name] = currentCount;
					return currentCount;
				}
				else
				{
					map.Add(name, 0);
					return 0;
				}
			}
		}
	}
}

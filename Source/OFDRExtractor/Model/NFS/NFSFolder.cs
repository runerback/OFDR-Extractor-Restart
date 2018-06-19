using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

		private void RemoveLastFile()
		{
			var files = this.files;
			var count = files.Count;
			if (count == 0)
				throw new InvalidOperationException("cannot remove file from empty sequence");
			files.RemoveAt(count - 1);
		}

		private List<NFSFolder> folders = new List<NFSFolder>();
		public IEnumerable<NFSFolder> Folders
		{
			get { return this.folders; }
		}

		private NFSFolder parentFolder = null;
		public NFSFolder ParentFolder
		{
			get { return this.parentFolder; }
		}

		public void Add(NFSFolder folder)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			if (this.folders.Contains(folder)) return;

			folder.parentFolder = this;
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

		public static Task<NFSFolder> Load(IEnumerable<string> nfsLines, IProgressReporter reporter)
		{
			return Task.Factory.StartNew(() =>
			{
				bool report = reporter != null;
				if (report)
					reporter.Start("read nfs folders");

				var root = NFSFolder.CreateRoot();
				if (nfsLines == null || !nfsLines.Any())
				{
					if (report)
						reporter.Complete("nfs folders loaded");
					return root;
				}

				var reader = new NFSFoldersReader(nfsLines);
				if (report)
				{
					reader.ProgressChanged += (_, e) =>
					{
						reporter.Report(e.Percent, e.Status);
					};
				}

				foreach (var folder in reader.Read())
					root.Add(folder);

				if (report)
					reporter.Complete("nfs folders loaded");
				return root;
			});
		}

		class NFSFoldersReader : IProgressChanged
		{
			public NFSFoldersReader(IEnumerable<string> nfsLines)
			{
				this.nfsLines = nfsLines.ToArray();
			}

			private readonly string[] nfsLines;
			private readonly object readLock = new object();
			private readonly Dictionary<string, int> fileOrderMap = new Dictionary<string, int>();

			private readonly Regex folderRegex = new Regex(
					@"^(?<folder>[^.:]+)\s+0$",
					RegexOptions.Compiled);
			private readonly Regex fileRegex = new Regex(
					@"^(?<file>(?<filename>.*?)\.(?<extension>.*?))\s+(?<size>\d+)$",
					RegexOptions.Compiled);

			public IEnumerable<NFSFolder> Read()
			{
				lock (readLock)
				{
					this.fileOrderMap.Clear();
					return readLines();
				}
			}

			private IEnumerable<NFSFolder> readLines()
			{
				var lines = this.nfsLines;
				int total = lines.Length;
				double current = 0;

				var folderRegex = this.folderRegex;
				var fileRegex = this.fileRegex;

				int folderIndex = 0;
				int fileIndex = 0;

				NFSFolder currentFolder = null;
				string previousFolderName = null;
				string previousFileName = null;
				foreach (var line in lines)
				{
					raiseProgressChanged(current++ / total, string.Format("reading \"{0}\"", line));

					var folderMatch = folderRegex.Match(line);
					if (folderMatch.Success)
					{
						string folderName = folderMatch.Groups["folder"].Value;

						if (string.Equals(previousFolderName, folderName, StringComparison.OrdinalIgnoreCase))
							continue;
						previousFolderName = folderName;

						currentFolder = new NFSFolder(folderIndex++, folderName);
						yield return currentFolder;

						previousFileName = null;
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
						
						//if there are two same file one by one, unpack first one will get an empty file.
						if (string.Equals(previousFileName, name, StringComparison.OrdinalIgnoreCase))
						{
							Console.WriteLine(name);
							currentFolder.RemoveLastFile();
						}
						else
							previousFileName = name;
						
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

			public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
			private void raiseProgressChanged(double percent, string status)
			{
				if (this.ProgressChanged != null)
					this.ProgressChanged(this, new ProgressChangedEventArgs(percent, status));
			}
		}
	}
}

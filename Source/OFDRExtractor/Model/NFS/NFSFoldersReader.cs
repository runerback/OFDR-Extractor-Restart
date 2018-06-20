using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OFDRExtractor.Model
{
	public sealed partial class NFSFolder
	{
		class NFSFoldersReader : IProgressChanged
		{
			public NFSFoldersReader(IEnumerable<string> nfsLines)
			{
				this.nfsLines = nfsLines.ToArray();
			}

			private readonly string[] nfsLines;
			private readonly object readLock = new object();

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
						int order = getFileOrder(name, size);

						//if there are two same file one by one, unpack first one will get an empty file.
						if (string.Equals(previousFileName, name, StringComparison.OrdinalIgnoreCase))
						{
							Console.WriteLine(name);
							currentFolder.RemoveLastFile();
							order++; //the order is kept with first file, so increase here
						}
						else
							previousFileName = name;

						var file = new NFSFile(filename, extension, currentFolder, fileIndex++, order, name, size);
						currentFolder.Add(file);
						continue;
					}
				}
			}

			private readonly Dictionary<string, NFSFileOrderData> fileOrderMap =
				new Dictionary<string, NFSFileOrderData>();

			private int getFileOrder(string name, long size)
			{
				if (string.IsNullOrEmpty(name) || size <= 0)
					return 0;

				var map = this.fileOrderMap;
				var key = name.ToLower();

				NFSFileOrderData existsOrderData;
				if (map.TryGetValue(key, out existsOrderData))
				{
					//if two files have same name and size but different folder, 
					//it's just the same file in different folder,
					//so keep order (the unpacker can't find the 'second' file)
					//if they are in same folder, handle outside
					if (existsOrderData.Size != size)
						existsOrderData.IncreaseOrder();
					return existsOrderData.Order;
				}

				map.Add(key, new NFSFileOrderData(size));
				return 0;
			}

			public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
			private void raiseProgressChanged(double percent, string status)
			{
				if (this.ProgressChanged != null)
					this.ProgressChanged(this, new ProgressChangedEventArgs(percent, status));
			}
		}

		sealed class NFSFileOrderData
		{
			public NFSFileOrderData(long size)
			{
				if (size <= 0)
					throw new ArgumentException("size. should greater than 0");
				this.size = size;
			}

			private readonly long size;
			public long Size
			{
				get { return this.size; }
			}

			private int order = 0;
			public int Order
			{
				get { return this.order; }
			}

			public void IncreaseOrder()
			{
				order++;
			}
		}
	}
}

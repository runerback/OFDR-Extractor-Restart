using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OFDRExtractor.Model
{
	/// <summary>
	/// file info in win_000.nfs
	/// </summary>
	abstract class NFSLine : XmlSerializable
	{
		protected NFSLine() { }

		private int index;
		/// <summary>
		/// index of file or folder in win_000.nfs
		/// </summary>
		public int Index
		{
			get { return this.index; }
		}

		private int order;
		/// <summary>
		/// order of file or folder with same name in win_000.nfs
		/// </summary>
		/// <remarks>
		/// such as 'xxxfile'(0), 'xxxfile'(1). this will be used in dat.exe when extracting to locate a file.
		/// </remarks>
		public int Order
		{
			get { return this.order; }
		}

		private string name;
		/// <summary>
		/// name of file or folder in win_000.nfs
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		private long size;
		/// <summary>
		/// size of file in win_000.nfs. for folder, this value is always 0.
		/// </summary>
		public long Size
		{
			get { return this.size; }
		}

		public static NFSLine[] Read(IEnumerable<string> nfsLines)
		{
			if (nfsLines == null || !nfsLines.Any())
				return new NFSLine[0];
			return new NFSLinesReader(nfsLines).Read();
		}

		class NFSLinesReader
		{
			public NFSLinesReader(IEnumerable<string> nfsLines)
			{
				this.nfsLines = nfsLines;
			}

			private IEnumerable<string> nfsLines;
			private readonly object readLock = new object();
			private Dictionary<string, int> fileOrderMap = new Dictionary<string, int>();

			private readonly Regex folderRegex = new Regex(
					@"^(?<folder>[^.:]+)\s+0$",
					RegexOptions.Compiled);
			private readonly Regex fileRegex = new Regex(
					@"^(?<file>(?<filename>.*?)\.(?<extension>.*?))\s+(?<size>\d+)$",
					RegexOptions.Compiled);

			public NFSLine[] Read()
			{
				lock (readLock)
				{
					this.fileOrderMap.Clear();
					return readLines().ToArray();
				}
			}

			private IEnumerable<NFSLine> readLines()
			{
				var folderRegex = this.folderRegex;
				var fileRegex = this.fileRegex;

				int index = 0;
				NFSFolder currentFolder = null;
				foreach (var line in nfsLines)
				{
					var folderMatch = folderRegex.Match(line);
					if (folderMatch.Success)
					{
						string folderName = folderMatch.Groups["folder"].Value;
						currentFolder = new NFSFolder
						{
							index = index++,
							order = 0,
							name = folderName,
							size = 0
						};
						yield return currentFolder;
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

						var file = new NFSFile(filename, extension, currentFolder)
						{
							index = index++,
							order = order,
							name = name,
							size = size
						};
						currentFolder.AddFile(file);

						yield return file;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OFDRExtractor.Model
{
	[XmlRoot("folder")]
	public sealed class PreparedFolder : IName
	{
		public PreparedFolder()
		{
			this.Folders = new List<PreparedFolder>();
			this.Files = new List<PreparedFile>();
		}

		public PreparedFolder(string name)
			: this()
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			this.Name = name;
		}

		[XmlAttribute("name")]
		public string Name { get; set; }
		[XmlElement("folder")]
		public List<PreparedFolder> Folders { get; set; }
		[XmlElement("file")]
		public List<PreparedFile> Files { get; set; }

		public PreparedFolder Copy()
		{
			var folder = new PreparedFolder(this.Name);

			folder.Files.AddRange(this.Files.Select(item => new PreparedFile(item.Name)));
			folder.Folders.AddRange(this.Folders.Select(item => item.Copy()));

			return folder;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1} files, {2} folders",
				this.Name,
				this.Files.Count,
				this.Folders.Count);
		}

		public static PreparedFolder Load(string rootPath)
		{
			if (string.IsNullOrEmpty(rootPath))
				throw new ArgumentNullException("rootPath");
			if (!Directory.Exists(rootPath))
				throw new DirectoryNotFoundException(rootPath);
			return new PreparedRootFolderReader().Read(rootPath);
		}

		class PreparedRootFolderReader
		{
			public PreparedFolder Read(string rootPath)
			{
				if (string.IsNullOrEmpty(rootPath))
					throw new ArgumentNullException("rootPath");
				if (!Directory.Exists(rootPath))
					throw new DirectoryNotFoundException(rootPath);

				return read(rootPath);
			}

			private PreparedFolder read(string path)
			{
				var folder = new PreparedFolder(Path.GetFileName(path));

				folder.Files.AddRange(Directory.GetFiles(path)
					.Select(Path.GetFileName)
					.Select(item => new PreparedFile(item)));
				folder.Folders.AddRange(Directory.GetDirectories(path).Select(read));

				return folder;
			}
		}
	}
}

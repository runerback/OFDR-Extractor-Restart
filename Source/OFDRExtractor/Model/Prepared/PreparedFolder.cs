using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public static Task<PreparedFolder> Load(string rootPath, IProgressReporter reporter)
		{
			if (string.IsNullOrEmpty(rootPath))
				throw new ArgumentNullException("rootPath");
			if (!Directory.Exists(rootPath))
				throw new DirectoryNotFoundException(rootPath);
			return Task.Factory.StartNew(() =>
			{
				var reader = new PreparedRootFolderReader();
				bool report = reporter != null;
				if (report)
				{
					reporter.Start("read prepared folders");
					reader.ProgressChanged += (_, e) =>
					{
						reporter.Report(e.Percent, e.Status);
					};
				}
				var root = reader.Read(rootPath);

				if (report)
					reporter.Complete("prepared folders loaded");

				return root;
			});
		}

		class PreparedRootFolderReader : IProgressChanged
		{
			public PreparedFolder Read(string rootPath)
			{
				if (string.IsNullOrEmpty(rootPath))
					throw new ArgumentNullException("rootPath");
				if (!Directory.Exists(rootPath))
					throw new DirectoryNotFoundException(rootPath);

				int total = Directory.EnumerateDirectories(rootPath, "*", SearchOption.AllDirectories).Count();
				return read(rootPath, new Progress(total));
			}

			private PreparedFolder read(string path, Progress progress)
			{
				string folderName = Path.GetFileName(path);
				var folder = new PreparedFolder(folderName);
				raiseProgressChanged(progress.Next(), folderName);

				folder.Files.AddRange(Directory.GetFiles(path)
					.Select(Path.GetFileName)
					.Select(item => new PreparedFile(item)));
				foreach (var subPath in Directory.GetDirectories(path))
					folder.Folders.Add(read(subPath, progress));

				return folder;
			}

			class Progress
			{
				public Progress(int total)
				{
					this.total = total;
				}

				private int current = 0;
				public int Current
				{
					get { return this.current; }
				}

				private int total;
				public int Total
				{
					get { return this.total; }
				}

				public double Next()
				{
					return (double)(this.current++) / this.total;
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

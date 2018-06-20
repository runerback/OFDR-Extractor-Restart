using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFDRExtractor.Model
{
	public sealed partial class NFSFolder : NFSLine
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
	}
}

using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	sealed class FileData : SelectableBase
	{
		public FileData(NFSFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			this.source = file;

			this.name = file.Name;
			this.filename = file.Filename;
			this.extension = file.Extension;
			this.size = formatSize(file.Size);
		}

		private readonly NFSFile source;
		public NFSFile Source
		{
			get { return this.source; }
		}

		private string name;
		public string Name
		{
			get { return this.name; }
		}

		private string filename;
		public string Filename
		{
			get { return this.filename; }
		}

		private string extension;
		public string Extension
		{
			get { return this.extension; }
		}

		private string size;
		public string Size
		{
			get { return this.size; }
		}

		#region format file size

		private static readonly string[] file_size_units = new string[] { "Bytes", "KB", "MB", "GB" };

		private static string formatSize(long size)
		{
			var units = file_size_units;

			if (size <= 0)
				return 0 + units[0];

			int pow = (int)Math.Floor(Math.Log(size) / Math.Log(1024));
			if (pow >= units.Length)
				pow = units.Length - 1;

			double value = (double)size / Math.Pow(1024, pow);
			return Math.Round(value, 1) + units[pow];
		}

		#endregion format file size

	}
}

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
			this.size = file.Size + " bytes";
		}

		private readonly NFSFile source;
		public NFSFile Souce
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
	}
}

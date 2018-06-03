using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Model
{
	sealed class NFSFile : NFSLine
	{
		public NFSFile(string filename, string extension, NFSFolder folder)
		{
			this.filename = filename;
			this.extension = extension;
			this.folder = folder;
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

		private NFSFolder folder;
		public NFSFolder Folder
		{
			get { return this.folder; }
		}

		public override void ReadXml(System.Xml.XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Format("{0} -> {1}{2} {3}",
				this.folder.Name,
				this.Name,
				this.Order > 0 ? string.Format("({0})", this.Order) : null,
				this.Size);
		}
	}
}

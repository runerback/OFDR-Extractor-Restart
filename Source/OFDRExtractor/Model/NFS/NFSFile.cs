using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Model
{
	public sealed class NFSFile : NFSLine
	{
		public NFSFile(string filename, string extension, NFSFolder folder, int index, int order, string name, long size)
			: base(index, order, name, size)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");
			
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

		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("file");

			writer.WriteStartAttribute("name");
			writer.WriteValue(this.Name);
			writer.WriteEndAttribute();

			if (this.Order > 0)
			{
				writer.WriteStartAttribute("order");
				writer.WriteValue(this.Order);
				writer.WriteEndAttribute();
			}

			writer.WriteEndElement();
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

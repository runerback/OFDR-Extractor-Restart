using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Model
{
	sealed class NFSFolder : NFSLine
	{
		private List<NFSFile> files = new List<NFSFile>();
		public IEnumerable<NFSFile> Files
		{
			get { return this.files; }
		}

		public void AddFile(NFSFile file)
		{
			this.files.Add(file);
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
			return string.Format("{0} {1} files",
				this.Name,
				this.files.Count);
		}
	}
}

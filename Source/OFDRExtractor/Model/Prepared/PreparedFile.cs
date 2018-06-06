using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OFDRExtractor.Model
{
	[Obsolete]
	public sealed class PreparedFile : IName
	{
		public PreparedFile()
		{
		}

		public PreparedFile(string name)
			: this()
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			this.Name = name;
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}

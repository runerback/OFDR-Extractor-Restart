using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OFDRExtractor.Model
{
	abstract class XmlSerializable : IXmlSerializable
	{
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public abstract void ReadXml(System.Xml.XmlReader reader);

		public abstract void WriteXml(System.Xml.XmlWriter writer);
	}
}

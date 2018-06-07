using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OFDRExtractor.Business
{
	public sealed class FolderSerializer
	{
		public MemoryStream Serialize(Model.NFSFolder root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			var output = new MemoryStream();
			var doc = new XDocument();
			using (var writer = doc.CreateWriter())
			{
				root.WriteXml(writer);
			}
			doc.Save(output, SaveOptions.OmitDuplicateNamespaces);
			return output;
		}

		#region obsoleted

		/*
		public MemoryStream Serialize(Model.PreparedFolder root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			var serializer = new XmlSerializer(typeof(Model.PreparedFolder));
			var output = new MemoryStream();
			serializer.Serialize(output, root, getNamespaces());
			return output;
		}

		public Model.PreparedFolder Deserialize(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			var serializer = new XmlSerializer(typeof(Model.PreparedFolder));
			return serializer.Deserialize(stream) as Model.PreparedFolder;
		}
		*/

		#endregion obsoleted

		private XmlSerializerNamespaces getNamespaces()
		{
			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add("", "");
			return namespaces;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Model
{
	public sealed class PreparedFolderBranchNode
	{
		public PreparedFolderBranchNode(IEnumerable<string> nodes, string name)
		{
			if (nodes == null || !nodes.Any())
				throw new ArgumentNullException("nodes");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			this.name = name;
			this.hashCode = string.Join("/", nodes).GetHashCode();
		}

		private string name;
		public string Name
		{
			get { return this.name; }
		}

		private int hashCode;
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override string ToString()
		{
			return this.name;
		}
	}
}

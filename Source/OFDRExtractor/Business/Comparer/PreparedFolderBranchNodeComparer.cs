using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	public sealed class PreparedFolderBranchNodeComparer : IEqualityComparer<PreparedFolderBranchNode>
	{
		public bool Equals(PreparedFolderBranchNode x, PreparedFolderBranchNode y)
		{
			return x == null ? y == null : x.GetHashCode() == y.GetHashCode();
		}

		public int GetHashCode(PreparedFolderBranchNode obj)
		{
			if (obj == null) return 0;
			return obj.GetHashCode();
		}
	}
}

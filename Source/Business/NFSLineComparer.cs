using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	sealed class NFSLineComparer : IEqualityComparer<Model.NFSLine>
	{
		public bool Equals(Model.NFSLine x, Model.NFSLine y)
		{
			return x == null ? y == null : x.Index == y.Index;
		}

		public int GetHashCode(Model.NFSLine obj)
		{
			if (obj == null) return 0;
			return obj.Index;
		}
	}
}

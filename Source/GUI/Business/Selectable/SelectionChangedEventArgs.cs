using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class SelectionChangedEventArgs : EventArgs
	{
		public SelectionChangedEventArgs(SelectableType sourceType)
		{
			this.sourceType = sourceType;
		}

		private SelectableType sourceType;
		public SelectableType SourceType
		{
			get { return this.sourceType; }
		}
	}
}

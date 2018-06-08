using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class FileSelectionChangedEventArgs : EventArgs
	{
		public FileSelectionChangedEventArgs(bool isSelected)
		{
			this.isSelected = isSelected;
		}

		private bool isSelected;
		public bool IsSelected
		{
			get { return this.isSelected; }
		}
	}
}

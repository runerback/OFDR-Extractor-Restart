using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
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

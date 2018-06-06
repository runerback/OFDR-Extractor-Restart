using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	class SelectableBase : ViewModelBase, Business.ISelectable
	{
		protected SelectableBase() { }

		private bool isSelected;
		public bool IsSelected
		{
			get { return this.isSelected; }
			set
			{
				if (this.isSelected != value)
				{
					this.isSelected = value;
					NotifyPropertyChanged("IsSelected");
					RaiseIsSelectedChanged();
				}
			}
		}

		public event EventHandler IsSelectedChanged;
		private void RaiseIsSelectedChanged()
		{
			if (IsSelectedChanged != null)
				IsSelectedChanged(this, EventArgs.Empty);
		}
	}
}

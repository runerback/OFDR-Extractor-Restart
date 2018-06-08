using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	class SelectableBase : ViewModelBase, Business.ISelectable
	{
		protected SelectableBase()
		{
			//this.ShouldNotifySelectionChanged = true;
		}

		private bool? isSelected = false;
		public bool? IsSelected
		{
			get { return this.isSelected; }
			set
			{
				//changed from UI
				if (SetIsSelected(value))
					RaiseIsSelectedChanged();
			}
		}

		//changed from backend
		internal bool SetIsSelected(bool? value)
		{
			if (this.isSelected != value)
			{
				this.isSelected = value;
				NotifyPropertyChanged("IsSelected");
				return true;
			}
			return false;
		}

		public event EventHandler IsSelectedChanged;
		protected void RaiseIsSelectedChanged()
		{
			if (IsSelectedChanged != null)
				IsSelectedChanged(this, EventArgs.Empty);
		}
	}
}

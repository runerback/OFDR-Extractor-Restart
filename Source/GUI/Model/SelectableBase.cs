using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Model
{
	class SelectableBase : ViewModelBase
	{
		protected SelectableBase() { }

		private bool? isSelected = false;
		public bool? IsSelected
		{
			get { return this.isSelected; }
			set
			{
				if (SetIsSelected(value))
				{
					if (IsSelectedChanged != null)
						IsSelectedChanged(this, EventArgs.Empty);
				}
			}
		}

		public bool SetIsSelected(bool? value)
		{
			if (this.isSelected != value)
			{
				this.isSelected = value;
				NotifyPropertyChanged("IsSelected");

				onIsSelectedChanged(value ?? false);

				return true;
			}
			return false;
		}

		protected virtual void onIsSelectedChanged(bool isSelected) { }

		public event EventHandler IsSelectedChanged;
	}
}

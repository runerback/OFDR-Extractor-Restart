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
			this.ShouldNotifySelectionChanged = true;
		}

		private bool? isSelected = false;
		public bool? IsSelected
		{
			get { return this.isSelected; }
			set
			{
				if (this.isSelected != value)
				{
					this.isSelected = value;
					NotifyPropertyChanged("IsSelected");

					if (this.ShouldNotifySelectionChanged)
					{
						onIsSelectedChanged();
						RaiseIsSelectedChanged();
					}
				}
			}
		}

		internal bool ShouldNotifySelectionChanged { get; set; }

		public event EventHandler IsSelectedChanged;
		protected void RaiseIsSelectedChanged()
		{
			if (IsSelectedChanged != null)
				IsSelectedChanged(this, EventArgs.Empty);
		}

		protected virtual void onIsSelectedChanged() { }
	}
}

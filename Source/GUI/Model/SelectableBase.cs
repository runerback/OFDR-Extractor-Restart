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
			this.ShouldRaiseEvent = true;
		}

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

					onIsSelectedChanged();
					if (this.ShouldRaiseEvent)
						RaiseIsSelectedChanged();
				}
			}
		}

		internal bool ShouldRaiseEvent { get; set; }

		public event EventHandler IsSelectedChanged;
		protected void RaiseIsSelectedChanged()
		{
			if (IsSelectedChanged != null)
				IsSelectedChanged(this, EventArgs.Empty);
		}

		protected virtual void onIsSelectedChanged() { }
	}
}

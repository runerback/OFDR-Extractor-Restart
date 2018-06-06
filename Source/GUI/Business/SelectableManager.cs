using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class SelectableManager : ViewModelBase
	{
		public SelectableManager(Model.FolderData source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			var array = selectables.ToArray();
			foreach (var item in array)
				item.IsSelectedChanged += onIsSelectedChanged;
		}

		private readonly Model.FolderData source;
		
		private void onIsSelectedChanged(object sender, EventArgs e)
		{
			updateStatus();
		}

		private bool isAnySelected;
		public bool IsAnySelected
		{
			get { return this.isAnySelected; }
			private set
			{
				if (this.isAnySelected != value)
				{
					this.isAnySelected = value;
					NotifyPropertyChanged("IsAnySelected");
				}
			}
		}

		private bool isAllSelected;
		public bool IsAllSelected
		{
			get { return this.isAllSelected; }
			private set
			{
				if (this.isAllSelected != value)
				{
					this.isAllSelected = value;
					NotifyPropertyChanged("IsAllSelected");
				}
			}
		}

		private int selectedCount;
		public int SelectedCount
		{
			get { return this.selectedCount; }
			private set
			{
				if (this.selectedCount != value)
				{
					this.selectedCount = value;
					NotifyPropertyChanged("SelectedCount");
				}
			}
		}

		private readonly List<ISelectable> selectedItems = new List<ISelectable>();
		public IEnumerable<ISelectable> SelectedItems
		{
			get { return this.selectedItems; }
		}

		private void updateStatus()
		{
			//TODO:
			//get all selected items count, refresh selected items list
			//update selected status
			throw new NotImplementedException();
		}

		#region Dispose

		private bool disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				foreach (var item in this.selectables)
					item.IsSelectedChanged -= onIsSelectedChanged;
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion Dispose

	}
}

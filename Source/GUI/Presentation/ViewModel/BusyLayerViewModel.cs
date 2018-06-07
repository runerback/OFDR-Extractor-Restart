using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	sealed class BusyLayerViewModel : ViewModelBase, IBusyLayerController
	{
		private BusyLayerViewModel() { }

		private static readonly IBusyLayerController instance = new BusyLayerViewModel();
		public static IBusyLayerController Instance
		{
			get { return instance; }
		}

		#region IBusyLayerController
		
		private bool isBusy = false;
		public bool IsBusy
		{
			get { return this.isBusy; }
		}

		private void setIsBusy(bool value)
		{
			if (value != this.isBusy)
			{
				this.isBusy = value;
				NotifyPropertyChanged("IsBusy");
			}
		}

		void IBusyLayerController.Busy()
		{
			setIsBusy(true);
		}

		void IBusyLayerController.Idle()
		{
			setIsBusy(false);
		}

		#endregion IBusyLayerController

	}
}

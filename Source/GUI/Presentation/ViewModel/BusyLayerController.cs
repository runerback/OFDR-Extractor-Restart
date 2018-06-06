using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	sealed class BusyLayerController : ViewModelBase
	{
		private BusyLayerController() { }

		private static readonly BusyLayerController instance = new BusyLayerController();
		public static BusyLayerController Instance
		{
			get { return instance; }
		}

		private bool isBusy = false;
		public bool IsBusy
		{
			get { return this.isBusy; }
			private set
			{
				if (value != this.isBusy)
				{
					this.isBusy = true;
					NotifyPropertyChanged("IsBusy");
				}
			}
		}

		public void Busy()
		{
			this.IsBusy = true;
		}

		public void Idle()
		{
			this.IsBusy = false;
		}
	}
}

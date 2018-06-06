using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			else
				Console.WriteLine("binding error: no such \"{0}\" property was bound in type {1}",
					propertyName,
					this.GetType().FullName);
		}
	}
}

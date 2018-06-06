using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OFDRExtractor.GUI.Business
{
	interface IPopupController
	{
		string Title { get; }
		string Message { get; }
		string Details { get; }
		bool HasDetails { get; }
		int PageIndex { get; }
		ICommand ShowDetailCommand { get; }
		ICommand HideDetailCommand { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface IBusyLayerController
	{
		void Busy();
		void Idle();
		bool IsBusy { get; }
	}
}

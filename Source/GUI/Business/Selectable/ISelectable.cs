using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	interface ISelectable
	{
		bool IsSelected { get; set; }
		event EventHandler IsSelectedChanged;
	}
}

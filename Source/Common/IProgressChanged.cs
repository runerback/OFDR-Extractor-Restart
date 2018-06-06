using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor
{
	public interface IProgressChanged
	{
		event EventHandler<ProgressChangedEventArgs> ProgressChanged;
	}
}

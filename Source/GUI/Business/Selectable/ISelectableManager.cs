using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface ISelectableManager : IDisposable
	{
		bool IsAnySelected { get; }
		bool IsAllSelected { get; }
		int SelectedFilesCount { get; }
		IEnumerable<Model.FileData> SelectedFiles { get; }
		event EventHandler SelectionChanged;
	}
}

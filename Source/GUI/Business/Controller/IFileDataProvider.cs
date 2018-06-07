using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface IFileDataProvider
	{
		IEnumerable<Model.FileData> AllFiles { get; }
		IEnumerable<Model.FileData> SelectedFiles { get; }
		int SelectedFilesCount { get; }
		event EventHandler SelectedFilesCountChanged;
	}
}

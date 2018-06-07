using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface IFileDataController
	{
		IToolBarController ToolBarController { get; }
		IEnumerable<Model.FolderData> RootSource { get; }
	}
}

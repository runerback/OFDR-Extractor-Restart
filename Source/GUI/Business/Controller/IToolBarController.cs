using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	interface IToolBarController
	{
		System.Windows.Input.ICommand RefreshCommand { get; }
		System.Windows.Input.ICommand UnpackAllCommand { get; }
		System.Windows.Input.ICommand UnpackSelectionCommand { get; }
		int SelectedFilesCount { get; }
	}
}

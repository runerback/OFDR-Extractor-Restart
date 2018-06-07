using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OFDRExtractor.GUI.Controls
{
	sealed class FileDataPresenter : Control
	{
		public FileDataPresenter()
		{

		}

		public Model.FileData Data
		{
			get { return (Model.FileData)this.GetValue(DataProperty); }
			set { this.SetValue(DataProperty, value); }
		}

		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register(
				"Data",
				typeof(Model.FileData),
				typeof(FileDataPresenter));

	}
}

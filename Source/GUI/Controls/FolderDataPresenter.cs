using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OFDRExtractor.GUI.Controls
{
	sealed class FolderDataPresenter : Control
	{
		public FolderDataPresenter()
		{

		}

		public Model.FolderData Data
		{
			get { return (Model.FolderData)this.GetValue(DataProperty); }
			set { this.SetValue(DataProperty, value); }
		}

		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register(
				"Data",
				typeof(Model.FolderData),
				typeof(FolderDataPresenter));

	}
}

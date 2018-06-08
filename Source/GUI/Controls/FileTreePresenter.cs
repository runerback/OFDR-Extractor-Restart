using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OFDRExtractor.GUI.Controls
{
	sealed class FileTreePresenter : Control
	{
		public FileTreePresenter()
		{

		}

		public IEnumerable<Model.FolderData> Source
		{
			get { return (IEnumerable<Model.FolderData>)this.GetValue(SourceProperty); }
			set { this.SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register(
				"Source",
				typeof(IEnumerable<Model.FolderData>),
				typeof(FileTreePresenter));

	}
}

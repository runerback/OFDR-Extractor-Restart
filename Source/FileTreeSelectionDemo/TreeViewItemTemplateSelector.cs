using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileTreeSelectionDemo
{
	sealed class TreeViewItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate File { get; set; }
		public DataTemplate Folder { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is FolderData)
				return Folder;
			else if (item is FileData)
				return File;
			else
				return base.SelectTemplate(item, container);
		}
	}
}

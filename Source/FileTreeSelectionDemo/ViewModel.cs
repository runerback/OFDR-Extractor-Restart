using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeSelectionDemo
{
	sealed class ViewModel : ViewModelBase
	{
		public ViewModel()
		{
			var root =  FileTreeBuilder.NFSRoot;
			this.root = root;
			this.helper = new SelectableHelper(root);
			this.helper.SelectedFileCountChanged += onSelectedFileCountChanged;
		}

		private readonly FolderData root;
		public IEnumerable<FolderData> Source
		{
			get { yield return root; }
		}

		private readonly SelectableHelper helper;

		public int SelectedFileCount
		{
			get { return helper.SelectedFileCount; }
		}

		private void onSelectedFileCountChanged(object sender, EventArgs e)
		{
			NotifyPropertyChanged("SelectedFileCount");
		}

	}
}

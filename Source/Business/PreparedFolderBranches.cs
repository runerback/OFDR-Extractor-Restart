using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	/// <summary>
	/// all branches start from layer 1 (direct sub folder of data_win)
	/// </summary>
	public sealed class PreparedFolderBranches
	{
		public PreparedFolderBranches(PreparedFolder root)
		{
			if (root == null)
				throw new ArgumentNullException("root");
			this.branches = BuildBranches(root).ToArray();
		}

		private readonly IEnumerable<PreparedFolderBranch> branches;
		public IEnumerable<PreparedFolderBranch> Branches
		{
			get { return this.branches; }
		}

		private IEnumerable<PreparedFolderBranch> BuildBranches(PreparedFolder folder)
		{
			Stack<PreparedFolder> folderStack = new Stack<PreparedFolder>();
			Stack<IEnumerator<PreparedFolder>> iteratorStack = new Stack<IEnumerator<PreparedFolder>>();

			iteratorStack.Push(folder.Folders.GetEnumerator());
			bool pushed = true;

			while (folderStack.Count > 0 || iteratorStack.Count > 0)
			{
				var iterator = iteratorStack.Peek();

				if (!iterator.MoveNext())
				{
					if (pushed)
					{
						yield return new PreparedFolderBranch(folderStack
							.Select(item => item.Name)
							.Reverse());
					}

					if (folderStack.Count > 0)
						folderStack.Pop();
					iteratorStack.Pop();
					iterator.Dispose();
					pushed = false;
				}
				else
				{
					iteratorStack.Push(iterator.Current.Folders.GetEnumerator());
					folderStack.Push(iterator.Current);
					pushed = true;
				}
			}
		}

		public PreparedFolderBranchRefMap CreateRefMap()
		{
			return new PreparedFolderBranchRefMap(this.branches);
		}
	}
}

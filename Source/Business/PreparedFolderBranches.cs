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
		#region obsolated

		/*
		public PreparedFolderBranches(PreparedFolder root)
		{
			if (root == null)
				throw new ArgumentNullException("root");
			this.branches = BuildBranches(root).ToArray();
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
		*/

		#endregion obsolated

		public PreparedFolderBranches(IEnumerable<string> branches)
		{
			this.branches = validate(BuildBranches(branches));
		}

		private readonly IEnumerable<PreparedFolderBranch> branches;
		public IEnumerable<PreparedFolderBranch> Branches
		{
			get { return this.branches; }
		}

		private IEnumerable<PreparedFolderBranch> BuildBranches(IEnumerable<string> branches)
		{
			if (branches == null || !branches.Any())
				yield break;
			foreach (var branch in branches)
				yield return new PreparedFolderBranch(branch);
		}

		//validate same node in differenct branch, one branch is contained in another branch compare from start
		//such as `a/b/c` and `a/b', not 'a/b/c/d` and 'a/b/c/e', not 'b/c' and 'a/b/c'
		private PreparedFolderBranch[] validate(IEnumerable<PreparedFolderBranch> branches)
		{
			var items = branches.ToArray();

			if (items.Length == 0) 
				return items;

			using (var branchIterator = branches
				.OrderBy(item => item.FullPath)
				.GetEnumerator())
			{
				branchIterator.MoveNext();
				var previous = branchIterator.Current;
				while (branchIterator.MoveNext())
				{
					var current = branchIterator.Current;
					if (current.IsStartsWith(previous.FullPath))
						throw new ArgumentException(
							string.Format("reduplicated branches : {0} - {1}", previous.FullPath, current.FullPath));
					previous = current;
				}
			}

			return items;
		}

		public PreparedFolderBranchRefMap CreateRefMap()
		{
			return new PreparedFolderBranchRefMap(this.branches);
		}
	}
}

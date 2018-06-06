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
	public sealed class NFSFolderBranchesManager
	{
		#region obsolated

		#endregion obsolated

		public NFSFolderBranchesManager(IEnumerable<string> branches, IProgressReporter reporter)
		{
			this.branches = validate(BuildBranches(branches), reporter);
		}

		private readonly IEnumerable<NFSFolderBranch> branches;
		public IEnumerable<NFSFolderBranch> Branches
		{
			get { return this.branches; }
		}

		private IEnumerable<NFSFolderBranch> BuildBranches(IEnumerable<string> branches)
		{
			if (branches == null || !branches.Any())
				yield break;
			foreach (var branch in branches)
				yield return new NFSFolderBranch(branch);
		}

		//validate same node in differenct branch, one branch is contained in another branch compare from start
		//such as `a/b/c` and `a/b', not 'a/b/c/d` and 'a/b/c/e', not 'b/c' and 'a/b/c'
		private NFSFolderBranch[] validate(IEnumerable<NFSFolderBranch> branches, IProgressReporter reporter)
		{
			bool report = reporter != null;

			var items = branches.ToArray();

			if(report)
				reporter.Report(0, "reading branches");

			int total = items.Length;

			if (total == 0)
			{
				if (report)
					reporter.Report(1, "branches loaded");
				return items;
			}

			int currentIndex = 0;

			using (var branchIterator = branches
				.OrderBy(item => item.FullPath)
				.GetEnumerator())
			{
				branchIterator.MoveNext();
				var previous = branchIterator.Current;

				if (report)
					reporter.Report(
						(double)(currentIndex++) / total,
						string.Format("reading branch \"{0}\"", previous.FullPath));

				while (branchIterator.MoveNext())
				{
					var current = branchIterator.Current;

					if (report)
						reporter.Report(
							(double)(currentIndex++) / total,
							string.Format("reading branch \"{0}\"", current.FullPath));

					if (current.IsStartsWith(previous.FullPath))
					{
						if (report)
							reporter.Report(0, null);
						throw new ArgumentException(
							string.Format("reduplicated branches : {0} - {1}", previous.FullPath, current.FullPath));
					}
					previous = current;
				}
			}

			if (report)
				reporter.Report(1, "branches loaded");
			return items;
		}

		internal NFSFolderBranchRefMap CreateRefMap()
		{
			return new NFSFolderBranchRefMap(this.branches);
		}
	}
}

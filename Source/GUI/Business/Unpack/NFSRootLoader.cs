using OFDRExtractor.Business;
using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFDRExtractor.GUI.Business.Unpack
{
	sealed class NFSRootLoader : IDisposable
	{
		public NFSRootLoader(IProgressReporter reporter)
		{
			if (!File.Exists(branchesFile))
				throw new FileNotFoundException("branches.txt");

			this.report = reporter != null;
			this.reporter = reporter;

			AppExitingHandler.AppExiting += onAppExiting;
		}

		private readonly string branchesFile = Directory.GetCurrentDirectory() + @"\branches.txt";

		private readonly bool report;
		private readonly IProgressReporter reporter;
		private static readonly object loadingLock = new object();

		public Model.FolderData LoadNFSRoot()
		{
			lock (loadingLock)
			{
				var report = this.report;
				var reporter = this.reporter;

				var nfsFolders = this.LoadNFSFolders();
				var branchesManager = this.LoadBranches();
				var root = this.BuildNFSRoot(nfsFolders, branchesManager);

				return new Model.FolderData(root);
			}
		}

		#region operations

		private NFSFolder[] LoadNFSFolders()
		{
			var report = this.report;
			var reporter = this.reporter;

			throw new NotImplementedException();
		}

		private NFSFolderBranchesManager LoadBranches()
		{
			var report = this.report;
			var reporter = this.reporter;

			throw new NotImplementedException();
		}

		private NFSFolder BuildNFSRoot(IEnumerable<NFSFolder> nfsFolders, NFSFolderBranchesManager branchesManager)
		{
			if (nfsFolders == null)
				throw new ArgumentNullException("nfsFolders");
			if (branchesManager == null)
				throw new ArgumentNullException("branchesManager");

			var report = this.report;
			var reporter = this.reporter;

			throw new NotImplementedException();
		}

		#endregion operations

		#region Dispose

		private void onAppExiting(object sender, EventArgs e)
		{
			Dispose(true);
		}

		private bool disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{

			}
			this.disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion Dispose

	}
}

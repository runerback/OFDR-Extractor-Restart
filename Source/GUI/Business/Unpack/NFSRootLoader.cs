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
			this.report = reporter != null;
			this.reporter = reporter;

			AppExitingHandler.AppExiting += onAppExiting;
		}

		private readonly string branchesFile = Directory.GetCurrentDirectory() + @"\branches.txt";

		private readonly bool report;
		private readonly IProgressReporter reporter;
		private static readonly object loadingLock = new object();

		public Model.FolderData BuildNFSRoot()
		{
			lock (loadingLock)
			{
				var report = this.report;
				var reporter = this.reporter;

				var nfsFolderRoot = this.LoadNFSFolderRoot();
				var branchesManager = this.LoadBranches();

				var builder = new NFSTreeBuilder(nfsFolderRoot, branchesManager);
				var root = builder.Build(reporter);

				return new Model.FolderData(root, null);
			}
		}

		#region operations

		private NFSFolder LoadNFSFolderRoot()
		{
			var reporter = this.reporter;

			string linesFile = "lines.txt";
			if (!File.Exists(linesFile))
				throw new FileNotFoundException(linesFile);
			var lines = File.ReadAllLines(linesFile);
			return NFSFolder.Load(lines, reporter).Result;
		}

		private NFSFolderBranchesManager LoadBranches()
		{
			var reporter = this.reporter;

			if (!File.Exists(branchesFile))
				throw new FileNotFoundException("branches.txt");
			var branches = File.ReadAllLines(branchesFile);
			return new NFSFolderBranchesManager(branches, reporter);
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

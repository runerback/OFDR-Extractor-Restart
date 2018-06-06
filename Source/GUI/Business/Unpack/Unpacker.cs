using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business.Unpack
{
	sealed class Unpacker : IDisposable
	{
		public Unpacker(IProgressReporter reporter)
		{
			this.reporter = reporter;
			this.report = reporter != null;

			AppExitingHandler.AppExiting += onAppExiting;
		}

		private readonly bool report;
		private readonly IProgressReporter reporter;
		private static readonly object unpackLock = new object();

		public bool Unpack(IEnumerable<Model.FileData> files)
		{
			lock (unpackLock)
			{
				try
				{
					return unpackMethod(files);
				}
				catch
				{
					throw;
				}
				finally
				{
					if (report)
					{
						var reporter = this.reporter;

						reporter.Report(0);
						reporter.Report(null);
					}
				}
			}
		}

		private bool unpackMethod(IEnumerable<Model.FileData> files)
		{
			if (files == null || !files.Any())
				return false;

			throw new NotImplementedException();
		}

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

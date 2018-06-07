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

			var reporter = this.reporter;

			var exceptions = new List<Tuple<string, Exception>>();

			//TODO: UI blocked, create a task queue.
			var extractor = new OFDRExtractor.Business.NFSFileExtractor();
			foreach (var file in files)
			{
				try
				{
					extractor.Extract(file.Source, reporter).Wait();
				}
				catch(Exception exp)
				{
					exceptions.Add(Tuple.Create(file.Name, exp));
				}
			}

			if (exceptions.Count > 0)
			{
				var errorDetailBuilder = new StringBuilder();
				foreach (var exception in exceptions)
				{
					errorDetailBuilder.AppendLine(
						string.Format("file \"{0}\" unpack failed: {1}",
							exception.Item1,
							exception.Item2));
				}
				Popup.Show(
					System.Windows.Application.Current.Dispatcher,
					"Error occurred while unpack files",
					"click [More info] for details",
					errorDetailBuilder.ToString());
				return false;
			}
			return true;
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

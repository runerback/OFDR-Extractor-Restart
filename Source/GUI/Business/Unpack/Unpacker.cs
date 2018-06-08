using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		private bool working = false;

		private readonly bool report;
		private readonly IProgressReporter reporter;

		public Task Unpack(IEnumerable<Model.FileData> files)
		{
			if (working)
				throw new InvalidOperationException("operation is already running");
			this.working = true;

			var task = Task.Factory.StartNew(() => 
				unpackMethod(files));

			task.ContinueWith(t =>
			{
				this.working = false;
				if (report)
					reporter.Report(0, null);

				if (t.IsFaulted)
				{
					Popup.Show(
						System.Windows.Application.Current.Dispatcher,
						"Error occurred while unpack files",
						"click [More info] for details",
						new ExceptionWrapper(t.Exception).Details);
				}
			});

			return task;
		}

		private void unpackMethod(IEnumerable<Model.FileData> files)
		{
			if (files == null || !files.Any())
				return;

			var report = this.report;
			var reporter = this.reporter;

			var exceptions = new List<Tuple<string, Exception>>();

			int total = files.Count();
			int current = 0;

			var extractor = new OFDRExtractor.Business.NFSFileExtractor();
			foreach (var file in files)
			{
				if (this.disposed)
					return;

				if (report)
					reporter.Report(
						string.Format("extracting {0}, {1} remainder", file.Name, total - current));

				try
				{
					extractor.Extract(file.Source).Wait();
				}
				catch (Exception exp)
				{
					if (report)
						reporter.Report("extract failed: " + file.Name);

					exceptions.Add(Tuple.Create(file.Name, exp));
				}

				if (report)
					reporter.Report((double)(current++) / total);
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
				throw new Exception(errorDetailBuilder.ToString());
			}
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

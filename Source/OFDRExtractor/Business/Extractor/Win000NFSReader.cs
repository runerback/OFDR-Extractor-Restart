using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OFDRExtractor.Business
{
	public sealed class Win000NFSReader : IDisposable
	{
		public Win000NFSReader()
		{
			this.invoker.ExtractorInvoked += onExtractorInvoked;
		}

		private readonly ExtractorInvoker invoker = new ExtractorInvoker();

		private InvokeResult result = null;
		private AutoResetEvent nfsReadBlock;
		private bool working = false;

		public Task<IEnumerable<string>> Read(IProgressReporter reporter)
		{
			if (working)
				throw new InvalidOperationException("operation is already running");

			bool report = reporter != null;

			if (report)
				reporter.Start("loading nfs lines");

			this.invoker.Invoke(null);
			this.nfsReadBlock = new AutoResetEvent(false);
			return Task.Factory.StartNew<IEnumerable<string>>(() =>
			{
				this.nfsReadBlock.WaitOne();
				this.nfsReadBlock.Dispose();
				this.nfsReadBlock = null;

				this.working = false;

				var result = this.result;
				if (result.HasError)
				{
					if (report)
						reporter.Report(0, null);
					throw new Exception("error occurred while loading nfs lines: " + result.Error);
				}

				if (report)
					reporter.Complete("nfs lines loaded");

				var lines = result.NfsLines;
				this.result = null;
				return lines;
			});
		}

		private void onExtractorInvoked(object sender, ExtractorInvokedEventArgs e)
		{
			if (e.HasError)
				this.result = new InvokeResult(e.Error);
			else
				this.result = new InvokeResult(e.HasOutput ? e.Output : Enumerable.Empty<string>());
			this.nfsReadBlock.Set();
		}

		class InvokeResult
		{
			public InvokeResult(IEnumerable<string> lines)
			{
				if (lines == null)
					throw new ArgumentNullException("lines");
				this.nfsLines = lines.ToArray();
			}

			public InvokeResult(string error)
			{
				this.error = error;
				this.hasError = true;
			}

			private string[] nfsLines;
			public string[] NfsLines
			{
				get { return this.nfsLines; }
			}

			private string error;
			public string Error
			{
				get { return this.error; }
			}

			private bool hasError = false;
			public bool HasError
			{
				get { return this.hasError; }
			}
		}
		
		#region Dispose

		private void onDomainExiting(object sender, EventArgs e)
		{
			this.Dispose(true);
		}

		private bool disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				if (this.nfsReadBlock != null)
				{
					this.nfsReadBlock.Set();
					this.nfsReadBlock.Dispose();
					this.nfsReadBlock = null;
				}
				this.invoker.Dispose();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OFDRExtractor.Business
{
	sealed class Win000NFSReader
	{
		public Win000NFSReader()
		{
			this.invoker.ExtractorInvoked += onExtractorInvoked;
		}

		private readonly ExtractorInvoker invoker = new ExtractorInvoker();

		private IEnumerable<string> nfsLines = null;
		private AutoResetEvent nfsReadBlock;
		private bool reading = false;

		public Task<IEnumerable<string>> Read()
		{
			if (reading)
				throw new InvalidOperationException("reading");

			this.invoker.Invoke(null);
			this.nfsReadBlock = new AutoResetEvent(false);
			return Task.Factory.StartNew<IEnumerable<string>>(() =>
			{
				this.nfsReadBlock.WaitOne();
				this.nfsReadBlock.Dispose();
				this.nfsReadBlock = null;

				var result = (this.nfsLines ?? Enumerable.Empty<string>()).ToArray();
				this.nfsLines = null;
				return result;
			});
		}

		private void onExtractorInvoked(object sender, ExtractorInvokedEventArgs e)
		{
			if (e.HasError)
				throw new Exception("extractor. " + e.Error);
			else
				this.nfsLines = e.HasOutput ? e.Output : null;
			this.nfsReadBlock.Set();
		}
	}
}

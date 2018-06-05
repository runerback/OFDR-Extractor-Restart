using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OFDRExtractor.GUI
{
	static class OFDRUnpacker
	{
		public static void Unpack(IProgressReporter reporter)
		{
			System.Threading.ThreadPool.QueueUserWorkItem(state => 
				new Unpacker(reporter).Unpack());
		}

		class Unpacker
		{
			public Unpacker(IProgressReporter reporter)
			{
				this.reporter = reporter;
				this.report = reporter != null;
			}

			private readonly bool report;
			private readonly IProgressReporter reporter;
			private readonly string branchesFile = Directory.GetCurrentDirectory() + @"\branches.txt";
			private static readonly object unpackLock = new object();

			public void Unpack()
			{
				lock (unpackLock)
				{
					try
					{
						unpackMethod();

						if (report)
						{
							var reporter = this.reporter;

							reporter.Report(100);
							reporter.Report("done");
						}
					}
					catch
					{
						if (report)
						{
							var reporter = this.reporter;

							reporter.Report(0);
							reporter.Report(null);
						}
						throw;
					}
				}
			}

			private void unpackMethod()
			{
				UITest();
			}

			private void UITest()
			{
				var report = this.report;
				var reporter = this.reporter;

				for (int i = 0; i < 10; i++)
				{
					if (report)
					{
						reporter.Report((double)i / 10);
						reporter.Report(string.Format("{0} remind", 10 - i));
					}
					System.Threading.Thread.Sleep(1000);
				}
			}
		}
	}
}

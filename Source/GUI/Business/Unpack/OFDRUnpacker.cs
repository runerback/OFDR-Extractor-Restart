using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

namespace OFDRExtractor.GUI
{
	static class OFDRUnpacker
	{
		private static bool working = false;

		public static Task<Model.FolderData> Load(IProgressReporter reporter)
		{
			var loader = new Business.Unpack.NFSRootLoader(reporter);
			return startOperation<Model.FolderData>(
				reporter,
				loader.LoadNFSRoot,
				loader);
		}

		public static Task Unpack(IProgressReporter reporter, IEnumerable<Model.FileData> files)
		{
			var unpacker = new Business.Unpack.Unpacker(reporter);
			return startOperation<bool>(
				reporter,
				() => unpacker.Unpack(files),
				unpacker);
		}

		private static Task<T> startOperation<T>(IProgressReporter reporter, Func<T> operation, IDisposable disposable)
		{
			if (operation == null)
				throw new ArgumentNullException("action");

			if (working)
				throw new InvalidOperationException("Unpack operation has already in running");
			working = true;
			BusyLayerController.Instance.Busy();

			var task = Task.Factory.StartNew<T>(() =>
			{
				UITest(reporter);
				return operation();
			});

			task.ContinueWith(t =>
			{
				if (disposable != null)
					disposable.Dispose();

				OFDRUnpacker.working = false;
				BusyLayerController.Instance.Idle();

				if (t.IsFaulted)
				{
					Popup.Show(Application.Current.Dispatcher, t.Exception);
					return;
				}
			});

			return task;
		}

		private static void UITest(IProgressReporter reporter)
		{
			var report = reporter != null;

			for (int i = 0; i < 10; i++)
			{
				if (report)
					reporter.Report((double)i / 10, string.Format("Tesing. . . {0} remind", 10 - i));
				System.Threading.Thread.Sleep(1000);
			}

			if (report)
				reporter.Complete("done");
		}
	}
}

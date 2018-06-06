using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI
{
	static class AppExitingHandler
	{
		static AppExitingHandler()
		{
			System.Windows.Application.Current.Exit += onAppExiting;
		}

		private static void onAppExiting(object sender, System.Windows.ExitEventArgs e)
		{
			if (AppExiting != null)
			{
				try
				{
					AppExiting(sender, e);
				}
				catch { } //TODO: add to log
			}
		}

		public static event EventHandler AppExiting;
	}
}

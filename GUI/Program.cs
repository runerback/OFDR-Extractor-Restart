using System;
using System.Windows;
using System.Windows.Threading;

namespace OFDRExtractor.GUI
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application app = new Application();
			app.DispatcherUnhandledException += onDispatcherUnhandledException;
			app.Run(new MainForm());
		}

		static void onDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			var exception = e.Exception;
			MessageBox.Show(exception.ToString(), exception.Message);
			e.Handled = true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OFDRExtractor.GUI
{
	partial class App : Application
	{
		public App()
		{
			this.DispatcherUnhandledException += onDispatcherUnhandledException;
			AppExitingHandler.AppExiting += onAppExiting;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			new ShellView().Show();
		}

		void onDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Popup.Show(e.Dispatcher, e.Exception);
			e.Handled = true;

			if (!this.MainWindow.IsVisible)
				Dispatcher.InvokeShutdown();
		}

		private void onAppExiting(object sender, EventArgs e)
		{
			//TODO: add to log
		}
	}
}

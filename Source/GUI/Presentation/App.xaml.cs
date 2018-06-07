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
			var shell = new ShellView();
			shell.ContentRendered += delegate
			{
				shell.DataContext = ShellViewModel.Instance;
			};
			shell.Show();
		}

		void onDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			try
			{
				Popup.Show(e.Dispatcher, e.Exception);
			}
			catch
			{
				//sometimes exception occurred before dispatcher run.
				Popup.Show(e.Exception); 
			}
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

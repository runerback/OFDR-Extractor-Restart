using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace OFDRExtractor.GUI
{
	static class Popup
	{
		#region Show

		public static void Show(string title, string message)
		{
			MessageBox.Show(message, title);
		}

		public static void Show(string message)
		{
			Show("Popup", message);
		}

		public static void Show(string title, string message, string details)
		{
			Show(title, message);
		}

		public static void Show(Exception e)
		{
			if (e == null) 
				return;

			var wrapped = new Business.ExceptionWrapper(e);
			Show("Error", wrapped.Message, wrapped.Details);
		}

		#endregion Show

		#region Show with dispatcher

		public static void Show(Dispatcher dispatcher, string title, string message)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			dispatcher.Invoke((Action)delegate
			{
				new Controls.Popup
				{
					DataContext = new PopupController(title, message)
				}.ShowDialog();
			});
		}

		public static void Show(Dispatcher dispatcher, string message)
		{
			Show(dispatcher, "Popup", message);
		}

		public static void Show(Dispatcher dispatcher, string title, string message, string details)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			dispatcher.Invoke((Action)delegate
			{
				new Controls.Popup
				{
					DataContext = new PopupController(title, message, details)
				}.ShowDialog();
			});
		}

		public static void Show(Dispatcher dispatcher, Exception e)
		{
			if (e == null) 
				return;

			var wrapped = new Business.ExceptionWrapper(e);
			Show(dispatcher, "Error", wrapped.Message, wrapped.Details);
		}
		
		#endregion Show with dispatcher

		sealed class PopupController : ViewModelBase, Business.IPopupController
		{
			private PopupController()
			{
				this.showDetailCommand = new SimpleCommand(showDetail);
				this.hideDetailCommand = new SimpleCommand(hideDetail);
			}

			public PopupController(string title, string message) : this()
			{
				this.title = title;
				this.message = message;
			}

			public PopupController(string title, string message, string details)
				: this(title, message)
			{
				this.details = details;
				this.hasDetails = !string.IsNullOrEmpty(details);
			}

			private string title;
			public string Title
			{
				get { return this.title; }
			}

			private string message;
			public string Message
			{
				get { return this.message; }
			}

			private string details;
			public string Details
			{
				get { return this.details; }
			}
			private bool hasDetails;
			public bool HasDetails
			{
				get { return this.hasDetails; }
			}
			
			private int pageIndex;
			public int PageIndex
			{
				get { return this.pageIndex; }
			}

			private SimpleCommand showDetailCommand;
			public ICommand ShowDetailCommand
			{
				get { return this.showDetailCommand; }
			}

			private void showDetail(object obj)
			{
				this.pageIndex = 1;
				NotifyPropertyChanged("PageIndex");
			}

			private SimpleCommand hideDetailCommand;
			public ICommand HideDetailCommand
			{
				get { return this.hideDetailCommand; }
			}

			private void hideDetail(object obj)
			{
				this.pageIndex = 0;
				NotifyPropertyChanged("PageIndex");
			}

		}
	}
}

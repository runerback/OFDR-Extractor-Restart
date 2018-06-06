using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace OFDRExtractor.GUI
{
	sealed class AutoInvokeObservableCollection<T> : ObservableCollection<T>
	{
		public AutoInvokeObservableCollection() : base() { }

		public AutoInvokeObservableCollection(IEnumerable<T> collection) : base(collection) { }

		public AutoInvokeObservableCollection(List<T> list) : base(list) { }

		public override event NotifyCollectionChangedEventHandler CollectionChanged;

		private bool IsRangeAdding = false;
		private readonly object RangeAddingLock = new object();

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (IsRangeAdding) return;

			var handler = this.CollectionChanged;
			if (handler != null)
			{
				foreach (NotifyCollectionChangedEventHandler invoker in handler.GetInvocationList())
				{
					DispatcherObject dispatcherObj = invoker.Target as DispatcherObject;
					if (dispatcherObj != null && dispatcherObj.Dispatcher != null)
					{
						Dispatcher dispatcher = dispatcherObj.Dispatcher;
						if (!dispatcher.CheckAccess())
						{
							dispatcher.Invoke((Action<NotifyCollectionChangedEventArgs>)this.OnCollectionChanged, DispatcherPriority.Background, e);
							continue;
						}
					}

					invoker.Invoke(this, e);
				}
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null) return;

			lock (RangeAddingLock)
			{
				this.IsRangeAdding = true;

				try
				{
					var addedItems = new List<T>();
					try
					{
						foreach (var item in collection)
						{
							Add(item);
							addedItems.Add(item);
						}
					}
					finally
					{
						if (addedItems.Count > 0)
						{
							OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems));
						}
					}
				}
				finally
				{
					this.IsRangeAdding = false;
				}
			}
		}
	}
}

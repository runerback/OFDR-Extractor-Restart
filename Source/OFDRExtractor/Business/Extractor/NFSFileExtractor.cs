using OFDRExtractor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OFDRExtractor.Business
{
	public sealed class NFSFileExtractor : IDisposable
	{
		public NFSFileExtractor()
		{
			this.invoker.ExtractorInvoked += onExtractorInvoked;
		}

		private readonly ExtractorInvoker invoker = new ExtractorInvoker();
		private readonly string rootPath = Directory.GetCurrentDirectory();

		private bool working = false;
		public bool Idle
		{
			get { return !this.working; }
		}

		public Task Extract(NFSFile file)
		{
			if (working)
				throw new InvalidOperationException("operation is already running");
			if (file == null)
				throw new ArgumentNullException("file");

			//return extractFile(file)
			//	.ContinueWith(t =>
			//	{
			//		if (t.IsFaulted)
			//			throw t.Exception.Flatten();

			//		string destFolder = checkDirectory(file);
			//		moveFile(file, destFolder);
			//	});

			string filename;
			if (file.Order > 0)
				filename = string.Format("{0} {1}", file.Name, file.Order + 1);
			else
				filename = file.Name;

			return unpackFile(filename)
				.ContinueWith(t =>
				{
					if (t.IsFaulted)
					{
						if (file.Order > 0)
						{
							try
							{
								//retry with default order 0
								unpackFile(file.Name).Wait();
							}
							catch
							{
								throw;
							}
						}
						else
						{
							throw t.Exception.Flatten();
						}
					}

					string destFolder = checkDirectory(file);
					moveFile(file, destFolder);
				});
		}

		#region extract file

		private InvokeResult result = null;
		private AutoResetEvent nfsReadBlock;

		[Obsolete("use `unpackFile` method instead")]
		private Task extractFile(NFSFile file)
		{
			//order start from 1 in extractor, and hide if it's first file.
			//such as for first xxx.xxx use origin name, for second use xxx.xxx 2
			
			//this.invoker.Invoke(
			//    string.Format("{0} {1}", file.Name, file.Order + 1));

			string filename;
			if (file.Order > 0)
				filename = string.Format("{0} {1}", file.Name, file.Order + 1);
			else
				filename = file.Name;

			return unpackFile(filename);
			//this.invoker.Invoke(filename);

			//this.nfsReadBlock = new AutoResetEvent(false);
			//return Task.Factory.StartNew(() =>
			//{
			//	this.nfsReadBlock.WaitOne();
			//	this.nfsReadBlock.Dispose();
			//	this.nfsReadBlock = null;

			//	this.working = false;

			//	var result = this.result;
			//	if (result.HasError)
			//		throw new Exception("error occurred while extract nfs file: " + result.Error);
			//});
		}

		private Task unpackFile(string filename)
		{
			this.invoker.Invoke(filename);

			this.nfsReadBlock = new AutoResetEvent(false);
			return Task.Factory.StartNew(() =>
			{
				this.nfsReadBlock.WaitOne();
				this.nfsReadBlock.Dispose();
				this.nfsReadBlock = null;

				this.working = false;

				var result = this.result;
				if (result.HasError)
					throw new Exception("error occurred while extract nfs file: " + result.Error);
			});
		}

		private void onExtractorInvoked(object sender, ExtractorInvokedEventArgs e)
		{
			if (e.HasError)
				this.result = new InvokeResult(e.Error);
			else
				this.result = new InvokeResult();
			this.nfsReadBlock.Set();
		}

		#endregion extract file

		private string checkDirectory(NFSFile file)
		{
			List<string> folders = new List<string>();
			var folder = file.Folder;
			while (folder != null)
			{
				folders.Add(folder.Name);
				folder = folder.ParentFolder;
			}
			string path = string.Join("\\", folders.Reverse<string>());

			string dest = Path.Combine(this.rootPath, path);
			if (!Directory.Exists(dest))
				Directory.CreateDirectory(dest);

			return dest;
		}

		//if file already in dest, it will be overwritten
		private void moveFile(NFSFile file, string destFolder)
		{
			string filename = file.Name;

			string order = null;
			if (file.Order > 0)
				order = (file.Order + 1).ToString();
			string sourceFile = Path.Combine(this.rootPath, filename + order);

			string destFile = Path.Combine(destFolder, filename);

			File.Copy(sourceFile, destFile, true);
			File.Delete(sourceFile);
		}

		class InvokeResult
		{
			public InvokeResult() { }

			public InvokeResult(string error)
			{
				this.error = error;
				this.hasError = true;
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

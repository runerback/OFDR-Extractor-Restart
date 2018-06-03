using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace OFDRExtractor.Business
{
	sealed class ExtractorInvoker : IDisposable
	{
		private static readonly string filename = Directory.GetCurrentDirectory() + @"\dat.exe";

		public ExtractorInvoker()
		{
			if (!File.Exists(filename))
				throw new FileNotFoundException(filename);
			Application.ApplicationExit += onApplicationExiting;
		}

		private Process currentProcess = null;

		public void Invoke(string command)
		{
			if (this.currentProcess != null) return;

			var startInfo = new ProcessStartInfo
			{
				FileName = filename,
				Arguments = command,
				WorkingDirectory = Path.GetDirectoryName(filename),
				CreateNoWindow = true,
				ErrorDialog = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			Process process = new Process
			{
				StartInfo = startInfo,
				EnableRaisingEvents = true
			};
			process.Exited += onProcessExiting;

			this.currentProcess = process;
			runProcess(process);

			//process.WaitForExit();
		}

		public event EventHandler<ExtractorInvokedEventArgs> ExtractorInvoked;

		private void onProcessExiting(object sender, EventArgs e)
		{
			var process = (Process)sender;

			//string error = process.StandardError.ReadToEnd();
			//string output = process.StandardOutput.ReadToEnd();

			//this.currentProcess = null;

			//string[] outputDatas = string.IsNullOrEmpty(output) ?
			//    null :
			//    output.Split(
			//        new string[] { Environment.NewLine },
			//        StringSplitOptions.RemoveEmptyEntries);

			//if (this.ExtractorInvoked != null)
			//{
			//    var args = new ExtractorInvokedEventArgs(outputDatas, error);
			//    this.ExtractorInvoked(this, args);
			//}

			process.CancelErrorRead();
			process.CancelOutputRead();

			this.currentProcess = null;

			var outputs = this.outputDatas.ToArray();
			string errors = this.errorBuilder.Length > 0 ? this.errorBuilder.ToString() : null;

			this.outputDatas.Clear();
			this.errorBuilder.Clear();

			if (this.ExtractorInvoked != null)
			{
				var args = new ExtractorInvokedEventArgs(outputs, errors);
				this.ExtractorInvoked(this, args);
			}
		}

		//private void onProcessDisposed(object sender, EventArgs e)
		//{
		//    this.currentProcess = null;

		//    var outputs = this.outputDatas.ToArray();
		//    string errors = this.errorBuilder.Length > 0 ? this.errorBuilder.ToString() : null;

		//    this.outputDatas.Clear();
		//    this.errorBuilder.Clear();

		//    if (this.ExtractorInvoked != null)
		//    {
		//        var args = new ExtractorInvokedEventArgs(outputs, errors);
		//        this.ExtractorInvoked(this, args);
		//    }
		//}

		private readonly StringBuilder errorBuilder = new StringBuilder();

		private void onErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
				return;
			this.errorBuilder.AppendLine(e.Data);
		}

		private readonly List<string> outputDatas = new List<string>();

		private void onOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
				return;
			this.outputDatas.Add(e.Data);
		}

		private void runProcess(Process process)
		{
			process.ErrorDataReceived += onErrorDataReceived;
			process.OutputDataReceived += onOutputDataReceived;

			process.Start();

			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
		}

		private void onApplicationExiting(object sender, EventArgs e)
		{
			this.Dispose(true);
		}

		private bool disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				if (this.currentProcess != null)
					this.currentProcess.Kill();
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}

	public sealed class ExtractorInvokedEventArgs : EventArgs
	{
		public ExtractorInvokedEventArgs(IEnumerable<string> output, string error)
		{
			this.output = output;
			this.hasOutput = output != null && output.Any();

			this.error = error;
			this.hasError = !string.IsNullOrEmpty(error);
		}

		private IEnumerable<string> output;
		public IEnumerable<string> Output
		{
			get { return this.output; }
		}

		private bool hasOutput;
		public bool HasOutput
		{
			get { return this.hasOutput; }
		}

		private string error;
		public string Error
		{
			get { return this.error; }
		}

		private bool hasError;
		public bool HasError
		{
			get { return this.hasError; }
		}
	}
}

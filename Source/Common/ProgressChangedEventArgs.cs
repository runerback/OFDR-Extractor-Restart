using System;

namespace OFDRExtractor
{
	public sealed class ProgressChangedEventArgs : EventArgs
	{
		public ProgressChangedEventArgs(double percent, string status)
		{
			if (percent < 0 || percent > 1)
				throw new ArgumentOutOfRangeException("percent. should within 0 to 1");
			this.percent = percent;
			this.status = status;
			this.hasPercentChanged = true;
			this.hasStatusChanged = true;
		}

		public ProgressChangedEventArgs(double percent)
		{
			if (percent < 0 || percent > 1)
				throw new ArgumentOutOfRangeException("percent. should within 0 to 1");
			this.percent = percent;
			this.hasPercentChanged = true;
		}

		public ProgressChangedEventArgs(string status)
		{
			this.status = status;
			this.hasStatusChanged = true;
		}

		private double percent = 0;
		public double Percent
		{
			get { return this.percent; }
		}

		private string status = null;
		public string Status
		{
			get { return this.status; }
		}

		private bool hasPercentChanged = false;
		public bool HasPercentChanged
		{
			get { return this.hasPercentChanged; }
		}

		private bool hasStatusChanged = false;
		public bool HasStatusChanged
		{
			get { return this.hasStatusChanged; }
		}
	}
}

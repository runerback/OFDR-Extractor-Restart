using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.UnitTest
{
	[TestClass]
	public class FileSizeFormatTest
	{
		[TestMethod]
		public void StartFileSizeFormatTest()
		{
			Assert.AreEqual(formatSize(128), "128 Bytes");
			Assert.AreEqual(formatSize(1024), "1 KB");
			Assert.AreEqual(formatSize(10240), "10 KB");
			Assert.AreEqual(formatSize(102400), "100 KB");
			Assert.AreEqual(formatSize(1048576), "1 MB");
			Assert.AreEqual(formatSize(111111), "108.5 KB");
			Assert.AreEqual(formatSize(1649738625), "1.5 GB");
		}

		private static readonly string[] file_size_units = new string[] { " Bytes", " KB", " MB", " GB" };

		private static string formatSize(long size)
		{
			var units = file_size_units;

			if (size <= 0)
				return 0 + units[0];

			int pow = (int)Math.Floor(Math.Log(size) / Math.Log(1024));
			if (pow >= units.Length)
				pow = units.Length - 1;

			double value = (double)size / Math.Pow(1024, pow);
			return Math.Round(value, 1) + units[pow];
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OFDRExtractor.Model
{
	/// <summary>
	/// file info in win_000.nfs
	/// </summary>
	public abstract class NFSLine : XmlSerializable, IName
	{
		protected NFSLine() { }

		protected NFSLine(int index, int order, string name, long size)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			this.index = index;
			this.order = order;
			this.name = name;
			this.size = size;
		}

		private int index;
		/// <summary>
		/// index of file or folder in win_000.nfs
		/// </summary>
		public int Index
		{
			get { return this.index; }
		}

		private int order;
		/// <summary>
		/// order of file or folder with same name in win_000.nfs. start from 0.
		/// </summary>
		/// <remarks>
		/// such as 'xxxfile'(0), 'xxxfile'(1). this will be used in dat.exe when extracting to locate a file.
		/// </remarks>
		public int Order
		{
			get { return this.order; }
		}

		private string name;
		/// <summary>
		/// name of file or folder in win_000.nfs
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		private long size;
		/// <summary>
		/// size of file in win_000.nfs. for folder, this value is always 0.
		/// </summary>
		public long Size
		{
			get { return this.size; }
		}

		public override void ReadXml(System.Xml.XmlReader reader)
		{
			throw new InvalidOperationException("write only");
		}
	}
}

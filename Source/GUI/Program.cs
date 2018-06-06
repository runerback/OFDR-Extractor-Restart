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
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}

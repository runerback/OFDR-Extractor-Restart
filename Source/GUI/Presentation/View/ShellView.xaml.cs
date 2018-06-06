﻿using System;
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

namespace OFDRExtractor.GUI
{
	partial class ShellView : Window
	{
		public ShellView()
		{
			InitializeComponent();

			this.DataContext = ShellViewModel.Instance;
			Dispatcher.BeginInvoke((Action)delegate
			{
				OFDRUnpacker.Unpack(ProgressReporterController.Instance, null);
			},
			System.Windows.Threading.DispatcherPriority.Loaded);
		}
	}
}
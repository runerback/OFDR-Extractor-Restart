using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OFDRExtractor.GUI.Controls
{
	sealed class WindowTitleBar : ContentControl
	{
		public WindowTitleBar()
		{

		}

		public bool ShowButton
		{
			get { return (bool)this.GetValue(ShowButtonProperty); }
			set { this.SetValue(ShowButtonProperty, value); }
		}

		public static readonly DependencyProperty ShowButtonProperty =
			DependencyProperty.Register(
				"ShowButton",
				typeof(bool),
				typeof(WindowTitleBar),
				new PropertyMetadata(true));
	}
}

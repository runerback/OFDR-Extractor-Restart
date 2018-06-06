using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OFDRExtractor.GUI.Controls
{
	sealed class ProgressReporter : Control
	{
		public double Progress
		{
			get { return (double)this.GetValue(ProgressProperty); }
			set { this.SetValue(ProgressProperty, value); }
		}

		public static readonly DependencyProperty ProgressProperty =
			DependencyProperty.Register(
				"Progress",
				typeof(double),
				typeof(ProgressReporter),
				new PropertyMetadata(onProgressPropertyChanged));

		private static void onProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var self = (ProgressReporter)d;
			self.SetProgressBarWidth(self.ActualWidth * (double)e.NewValue);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			if (sizeInfo.WidthChanged)
				SetProgressBarWidth(sizeInfo.NewSize.Width * Progress);
		}

		public double ProgressBarWidth
		{
			get { return (double)GetValue(ProgressBarWidthProperty); }
		}

		static readonly DependencyPropertyKey ProgressBarWidthPropertyKey =
			DependencyProperty.RegisterReadOnly(
				"ProgressBarWidth",
				typeof(double),
				typeof(ProgressReporter),
				new PropertyMetadata());

		public static readonly DependencyProperty ProgressBarWidthProperty =
			ProgressBarWidthPropertyKey.DependencyProperty;

		private void SetProgressBarWidth(double value)
		{
			var width = this.ActualWidth;
			SetValue(ProgressBarWidthPropertyKey, value);
		}

		public string Status
		{
			get { return (string)this.GetValue(StatusProperty); }
			set { this.SetValue(StatusProperty, value); }
		}

		public static readonly DependencyProperty StatusProperty =
			DependencyProperty.Register(
				"Status",
				typeof(string),
				typeof(ProgressReporter));

		public Brush ProgressBarBrush
		{
			get { return (Brush)this.GetValue(ProgressBarBrushProperty); }
			set { this.SetValue(ProgressBarBrushProperty, value); }
		}

		public static readonly DependencyProperty ProgressBarBrushProperty =
			DependencyProperty.Register(
				"ProgressBarBrush",
				typeof(Brush),
				typeof(ProgressReporter));
	}
}

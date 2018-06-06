using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.GUI.Business
{
	sealed class ExceptionWrapper
	{
		public ExceptionWrapper(Exception e)
		{
			if (e == null) 
				return;

			var messageBuilder = new StringBuilder();
			var detailBuilder = new StringBuilder();

			wrap(e, messageBuilder, detailBuilder);

			this.message = messageBuilder.ToString();
			this.details = detailBuilder.ToString();
		}

		private string message;
		public string Message
		{
			get { return this.message; }
		}

		private string details;
		public string Details
		{
			get { return this.details; }
		}

		private void wrap(Exception e, StringBuilder messageBuilder, StringBuilder detailBuilder)
		{
			if (e == null)
				return;

			Exception exp = e;
			int indent = 0;
			while (exp != null)
			{
				if (exp is AggregateException)
				{
					var flattenedAggregateException = ((AggregateException)exp).Flatten();
					foreach (Exception inner in flattenedAggregateException.InnerExceptions)
					{
						wrap(inner, messageBuilder, detailBuilder);
					}
					exp = null;
				}
				else if (exp is System.Reflection.TargetInvocationException)
				{
					exp = ((System.Reflection.TargetInvocationException)exp).InnerException;
				}
				else
				{
					messageBuilder.AppendLine(new string('\t', indent) + exp.Message);
					detailBuilder.AppendLine(new string('\t', indent) + exp.ToString());
					indent++;
					exp = exp.InnerException;
				}
			}
		}
	}
}

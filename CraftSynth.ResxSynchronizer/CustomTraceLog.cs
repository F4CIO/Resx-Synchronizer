using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftSynth.ResxSynchronizer
{
	/// <summary>
	/// Sole purpose of this class is to help to build string that represents processing steps.
	/// String can be included in ex message.
	/// 
	/// Result example:
	/// Started login process...
	///   Checking pass...
	///     Success..
	///   Checing roles..
	///     Sucess..
	/// Login done.
	/// </summary>
	public class CustomTraceLog
	{
		private StringBuilder sb;
		private int ident;
		private string Ident
		{
			get
			{
				string identPrefix = string.Empty;
				identPrefix = CalculateIdent(this.ident);
				return identPrefix;
			}
		}

		private string CalculateIdent(int ident)
		{
			string result = string.Empty;
			for (int i = 0; i < ident; i++)
			{
				result += "  ";
			}
			return result;
		}

		public CustomTraceLog(string initialValue = null)
		{
			sb = new StringBuilder(initialValue ?? string.Empty);
			ident = 0;
		}

		public void IncreaseIdent()
		{
			this.ident++;
		}

		public void DecreaseIdent()
		{
			if (this.ident > 0)
			{
				ident--;
			}
		}

		public void AddLine(string message)
		{
			sb.AppendLine(this.Ident + message);
		}

		public void ExtendLastLine(string message)
		{
			sb.Append(message);
		}

		public void AddLine(int ident, string message)
		{
			sb.AppendLine(this.CalculateIdent(ident) + message);
		}

		public override string ToString()
		{
			return sb.ToString();
		}
	}
}

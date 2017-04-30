using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CraftSynth.ResxSynchronizer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			string exeFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
			string readMeFilePath = Path.Combine(exeFolderPath, "ReadMe.txt");
			this.rtbReadMe.Lines = File.ReadAllLines(readMeFilePath);

			string iniFilePath = Path.Combine(exeFolderPath, "ResXSynchronizer.ini");
			if (File.Exists(iniFilePath))
			{
				var lines = File.ReadAllLines(iniFilePath);
				this.btnFolder1.Text = lines[0];
				this.btnFolder2.Text = lines[1];
				this.btnFolderBackup.Text = lines[2];
			}
		}

		private void btnFolder1_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = this.btnFolder1.Text;
			DialogResult r = folderBrowserDialog1.ShowDialog();
			if (r == DialogResult.OK)
			{
				this.btnFolder1.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void btnSynchronize_Click(object sender, EventArgs e)
		{
			if (this.btnFolder1.Text.IsNullOrWhiteSpace() || this.btnFolder2.Text.IsNullOrWhiteSpace()||this.btnFolderBackup.Text.IsNullOrWhiteSpace())
			{
				MessageBox.Show("Please set all three folders.");
			}else
			{
				string exeFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
				string iniFilePath = Path.Combine(exeFolderPath, "ResXSynchronizer.ini");
				string[] lines = new string[] { this.btnFolder1.Text, this.btnFolder2.Text, this.btnFolderBackup.Text };

				File.WriteAllLines(iniFilePath,lines);

				string log = string.Empty;
				bool hasErrors = Program.MainInner(new string[] {  this.btnFolder1.Text, this.btnFolder2.Text, this.btnFolderBackup.Text }, ref log);
				this.rtbLog.Text = log;
			}

		}

		private void btnFolder2_Click(object sender, EventArgs e)
		{
			folderBrowserDialog2.SelectedPath = this.btnFolder2.Text;
			DialogResult r = folderBrowserDialog2.ShowDialog();
			if (r == DialogResult.OK)
			{
				this.btnFolder2.Text = folderBrowserDialog2.SelectedPath;
			}
		}

		private void btnFolderBackup_Click(object sender, EventArgs e)
		{
			folderBrowserDialog3.SelectedPath = this.btnFolderBackup.Text;
			DialogResult r = folderBrowserDialog3.ShowDialog();
			if (r == DialogResult.OK)
			{
				this.btnFolderBackup.Text = folderBrowserDialog3.SelectedPath;
			}
		}

		private void btnRestoreLastBackup_Click(object sender, EventArgs e)
		{
			var hasErrors = Program.RestoreLastBackup(this.btnFolderBackup.Text, this.btnFolder1.Text, this.btnFolder2.Text);
			MessageBox.Show("Done. Has errors:" + hasErrors);
		}

		private void btnViewInNotepad_Click(object sender, EventArgs e)
		{
			var logFilePath = Program.GetLastLogFilePath(this.btnFolderBackup.Text);
			Program.OpenFile(logFilePath);
		}
	}
}

namespace CraftSynth.ResxSynchronizer
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnFolder1 = new System.Windows.Forms.Button();
			this.btnFolder2 = new System.Windows.Forms.Button();
			this.btnSynchronize = new System.Windows.Forms.Button();
			this.btnFolderBackup = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.rtbReadMe = new System.Windows.Forms.RichTextBox();
			this.rtbLog = new System.Windows.Forms.RichTextBox();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
			this.folderBrowserDialog3 = new System.Windows.Forms.FolderBrowserDialog();
			this.btnRestoreLastBackup = new System.Windows.Forms.Button();
			this.btnViewInNotepad = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnFolder1
			// 
			this.btnFolder1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnFolder1.Location = new System.Drawing.Point(13, 267);
			this.btnFolder1.Name = "btnFolder1";
			this.btnFolder1.Size = new System.Drawing.Size(866, 23);
			this.btnFolder1.TabIndex = 0;
			this.btnFolder1.UseVisualStyleBackColor = true;
			this.btnFolder1.Click += new System.EventHandler(this.btnFolder1_Click);
			// 
			// btnFolder2
			// 
			this.btnFolder2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnFolder2.Location = new System.Drawing.Point(12, 308);
			this.btnFolder2.Name = "btnFolder2";
			this.btnFolder2.Size = new System.Drawing.Size(867, 23);
			this.btnFolder2.TabIndex = 1;
			this.btnFolder2.UseVisualStyleBackColor = true;
			this.btnFolder2.Click += new System.EventHandler(this.btnFolder2_Click);
			// 
			// btnSynchronize
			// 
			this.btnSynchronize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnSynchronize.Location = new System.Drawing.Point(312, 394);
			this.btnSynchronize.Name = "btnSynchronize";
			this.btnSynchronize.Size = new System.Drawing.Size(269, 23);
			this.btnSynchronize.TabIndex = 2;
			this.btnSynchronize.Text = "Synchronize ResX Files";
			this.btnSynchronize.UseVisualStyleBackColor = true;
			this.btnSynchronize.Click += new System.EventHandler(this.btnSynchronize_Click);
			// 
			// btnFolderBackup
			// 
			this.btnFolderBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnFolderBackup.Location = new System.Drawing.Point(13, 352);
			this.btnFolderBackup.Name = "btnFolderBackup";
			this.btnFolderBackup.Size = new System.Drawing.Size(867, 23);
			this.btnFolderBackup.TabIndex = 4;
			this.btnFolderBackup.UseVisualStyleBackColor = true;
			this.btnFolderBackup.Click += new System.EventHandler(this.btnFolderBackup_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 251);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Folder 1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 292);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Folder 2";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 334);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(146, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Folder for backup and Log.txt";
			// 
			// rtbReadMe
			// 
			this.rtbReadMe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.rtbReadMe.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbReadMe.Location = new System.Drawing.Point(12, 13);
			this.rtbReadMe.Name = "rtbReadMe";
			this.rtbReadMe.ReadOnly = true;
			this.rtbReadMe.Size = new System.Drawing.Size(867, 222);
			this.rtbReadMe.TabIndex = 8;
			this.rtbReadMe.Text = "";
			this.rtbReadMe.WordWrap = false;
			// 
			// rtbLog
			// 
			this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.rtbLog.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbLog.Location = new System.Drawing.Point(13, 445);
			this.rtbLog.Name = "rtbLog";
			this.rtbLog.ReadOnly = true;
			this.rtbLog.Size = new System.Drawing.Size(867, 166);
			this.rtbLog.TabIndex = 9;
			this.rtbLog.Text = "";
			this.rtbLog.WordWrap = false;
			// 
			// btnRestoreLastBackup
			// 
			this.btnRestoreLastBackup.Location = new System.Drawing.Point(16, 394);
			this.btnRestoreLastBackup.Name = "btnRestoreLastBackup";
			this.btnRestoreLastBackup.Size = new System.Drawing.Size(155, 23);
			this.btnRestoreLastBackup.TabIndex = 10;
			this.btnRestoreLastBackup.Text = "Restore Last Backup";
			this.btnRestoreLastBackup.UseVisualStyleBackColor = true;
			this.btnRestoreLastBackup.Click += new System.EventHandler(this.btnRestoreLastBackup_Click);
			// 
			// btnViewInNotepad
			// 
			this.btnViewInNotepad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnViewInNotepad.Location = new System.Drawing.Point(733, 416);
			this.btnViewInNotepad.Name = "btnViewInNotepad";
			this.btnViewInNotepad.Size = new System.Drawing.Size(146, 23);
			this.btnViewInNotepad.TabIndex = 11;
			this.btnViewInNotepad.Text = "View Last Log in Notepad";
			this.btnViewInNotepad.UseVisualStyleBackColor = true;
			this.btnViewInNotepad.Click += new System.EventHandler(this.btnViewInNotepad_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(891, 623);
			this.Controls.Add(this.btnViewInNotepad);
			this.Controls.Add(this.btnRestoreLastBackup);
			this.Controls.Add(this.rtbLog);
			this.Controls.Add(this.rtbReadMe);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnFolderBackup);
			this.Controls.Add(this.btnSynchronize);
			this.Controls.Add(this.btnFolder2);
			this.Controls.Add(this.btnFolder1);
			this.Name = "Form1";
			this.Text = "ResxSynchronizer";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnFolder1;
		private System.Windows.Forms.Button btnFolder2;
		private System.Windows.Forms.Button btnSynchronize;
		private System.Windows.Forms.Button btnFolderBackup;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RichTextBox rtbReadMe;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog3;
		public System.Windows.Forms.RichTextBox rtbLog;
		private System.Windows.Forms.Button btnRestoreLastBackup;
		private System.Windows.Forms.Button btnViewInNotepad;
	}
}


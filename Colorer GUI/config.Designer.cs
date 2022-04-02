
namespace Colorer_GUI
{
	partial class config
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
			this.openFileDialogConfig = new System.Windows.Forms.OpenFileDialog();
			this.txtPath = new System.Windows.Forms.TextBox();
			this.btnSetPath = new System.Windows.Forms.Button();
			this.txtEndName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ckbxnoncontiguous = new System.Windows.Forms.CheckBox();
			this.ckbxExisting = new System.Windows.Forms.CheckBox();
			this.ckbxDebug = new System.Windows.Forms.CheckBox();
			this.ckbxWarningFile = new System.Windows.Forms.CheckBox();
			this.upDownProvRadius = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.grpContiguous = new System.Windows.Forms.GroupBox();
			this.upDownProvSize = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.btnExportConfig = new System.Windows.Forms.Button();
			this.btnImportConfig = new System.Windows.Forms.Button();
			this.btnSubmit = new System.Windows.Forms.Button();
			this.saveFileDialogConfig = new System.Windows.Forms.SaveFileDialog();
			this.folderBrowserDialogPath = new System.Windows.Forms.FolderBrowserDialog();
			((System.ComponentModel.ISupportInitialize)(this.upDownProvRadius)).BeginInit();
			this.grpContiguous.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.upDownProvSize)).BeginInit();
			this.SuspendLayout();
			// 
			// openFileDialogConfig
			// 
			this.openFileDialogConfig.FileName = "openFileDialogConfig";
			// 
			// txtPath
			// 
			this.txtPath.Enabled = false;
			this.txtPath.Location = new System.Drawing.Point(12, 32);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(274, 22);
			this.txtPath.TabIndex = 0;
			// 
			// btnSetPath
			// 
			this.btnSetPath.Location = new System.Drawing.Point(292, 32);
			this.btnSetPath.Name = "btnSetPath";
			this.btnSetPath.Size = new System.Drawing.Size(22, 23);
			this.btnSetPath.TabIndex = 1;
			this.btnSetPath.UseVisualStyleBackColor = true;
			this.btnSetPath.Click += new System.EventHandler(this.btnSetPath_Click);
			// 
			// txtEndName
			// 
			this.txtEndName.Location = new System.Drawing.Point(12, 77);
			this.txtEndName.Name = "txtEndName";
			this.txtEndName.Size = new System.Drawing.Size(144, 22);
			this.txtEndName.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "Path";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Output file name";
			// 
			// ckbxnoncontiguous
			// 
			this.ckbxnoncontiguous.AutoSize = true;
			this.ckbxnoncontiguous.Checked = true;
			this.ckbxnoncontiguous.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckbxnoncontiguous.Location = new System.Drawing.Point(253, 93);
			this.ckbxnoncontiguous.Name = "ckbxnoncontiguous";
			this.ckbxnoncontiguous.Size = new System.Drawing.Size(258, 21);
			this.ckbxnoncontiguous.TabIndex = 5;
			this.ckbxnoncontiguous.Text = "Non-contiguous province detection?";
			this.ckbxnoncontiguous.UseVisualStyleBackColor = true;
			this.ckbxnoncontiguous.CheckedChanged += new System.EventHandler(this.ckbxnoncontiguous_CheckedChanged);
			// 
			// ckbxExisting
			// 
			this.ckbxExisting.AutoSize = true;
			this.ckbxExisting.Checked = true;
			this.ckbxExisting.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckbxExisting.Location = new System.Drawing.Point(320, 39);
			this.ckbxExisting.Name = "ckbxExisting";
			this.ckbxExisting.Size = new System.Drawing.Size(143, 21);
			this.ckbxExisting.TabIndex = 6;
			this.ckbxExisting.Text = "Existing provinces";
			this.ckbxExisting.UseVisualStyleBackColor = true;
			// 
			// ckbxDebug
			// 
			this.ckbxDebug.AutoSize = true;
			this.ckbxDebug.Location = new System.Drawing.Point(320, 66);
			this.ckbxDebug.Name = "ckbxDebug";
			this.ckbxDebug.Size = new System.Drawing.Size(116, 21);
			this.ckbxDebug.TabIndex = 7;
			this.ckbxDebug.Text = "Debug output";
			this.ckbxDebug.UseVisualStyleBackColor = true;
			// 
			// ckbxWarningFile
			// 
			this.ckbxWarningFile.AutoSize = true;
			this.ckbxWarningFile.Checked = true;
			this.ckbxWarningFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckbxWarningFile.Location = new System.Drawing.Point(320, 12);
			this.ckbxWarningFile.Name = "ckbxWarningFile";
			this.ckbxWarningFile.Size = new System.Drawing.Size(191, 21);
			this.ckbxWarningFile.TabIndex = 8;
			this.ckbxWarningFile.Text = "Create debug warning file";
			this.ckbxWarningFile.UseVisualStyleBackColor = true;
			// 
			// upDownProvRadius
			// 
			this.upDownProvRadius.Location = new System.Drawing.Point(173, 18);
			this.upDownProvRadius.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
			this.upDownProvRadius.Name = "upDownProvRadius";
			this.upDownProvRadius.Size = new System.Drawing.Size(74, 22);
			this.upDownProvRadius.TabIndex = 9;
			this.upDownProvRadius.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(168, 17);
			this.label3.TabIndex = 10;
			this.label3.Text = "Province detection radius";
			// 
			// grpContiguous
			// 
			this.grpContiguous.Controls.Add(this.upDownProvSize);
			this.grpContiguous.Controls.Add(this.label4);
			this.grpContiguous.Controls.Add(this.upDownProvRadius);
			this.grpContiguous.Controls.Add(this.label3);
			this.grpContiguous.Location = new System.Drawing.Point(253, 111);
			this.grpContiguous.Name = "grpContiguous";
			this.grpContiguous.Size = new System.Drawing.Size(253, 72);
			this.grpContiguous.TabIndex = 11;
			this.grpContiguous.TabStop = false;
			// 
			// upDownProvSize
			// 
			this.upDownProvSize.Location = new System.Drawing.Point(173, 45);
			this.upDownProvSize.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.upDownProvSize.Name = "upDownProvSize";
			this.upDownProvSize.Size = new System.Drawing.Size(74, 22);
			this.upDownProvSize.TabIndex = 11;
			this.upDownProvSize.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 45);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(129, 17);
			this.label4.TabIndex = 12;
			this.label4.Text = "Small province size";
			// 
			// btnExportConfig
			// 
			this.btnExportConfig.Enabled = false;
			this.btnExportConfig.Location = new System.Drawing.Point(15, 111);
			this.btnExportConfig.Name = "btnExportConfig";
			this.btnExportConfig.Size = new System.Drawing.Size(109, 29);
			this.btnExportConfig.TabIndex = 12;
			this.btnExportConfig.Text = "Export Config";
			this.btnExportConfig.UseVisualStyleBackColor = true;
			this.btnExportConfig.Click += new System.EventHandler(this.btnExportConfig_Click);
			// 
			// btnImportConfig
			// 
			this.btnImportConfig.Enabled = false;
			this.btnImportConfig.Location = new System.Drawing.Point(130, 111);
			this.btnImportConfig.Name = "btnImportConfig";
			this.btnImportConfig.Size = new System.Drawing.Size(109, 29);
			this.btnImportConfig.TabIndex = 13;
			this.btnImportConfig.Text = "Inport Config";
			this.btnImportConfig.UseVisualStyleBackColor = true;
			// 
			// btnSubmit
			// 
			this.btnSubmit.Location = new System.Drawing.Point(15, 156);
			this.btnSubmit.Name = "btnSubmit";
			this.btnSubmit.Size = new System.Drawing.Size(224, 27);
			this.btnSubmit.TabIndex = 14;
			this.btnSubmit.Text = "Apply ";
			this.btnSubmit.UseVisualStyleBackColor = true;
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			// 
			// config
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(520, 195);
			this.Controls.Add(this.btnSubmit);
			this.Controls.Add(this.btnImportConfig);
			this.Controls.Add(this.btnExportConfig);
			this.Controls.Add(this.grpContiguous);
			this.Controls.Add(this.ckbxnoncontiguous);
			this.Controls.Add(this.ckbxWarningFile);
			this.Controls.Add(this.ckbxDebug);
			this.Controls.Add(this.ckbxExisting);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtEndName);
			this.Controls.Add(this.btnSetPath);
			this.Controls.Add(this.txtPath);
			this.Name = "config";
			this.Text = "config";
			((System.ComponentModel.ISupportInitialize)(this.upDownProvRadius)).EndInit();
			this.grpContiguous.ResumeLayout(false);
			this.grpContiguous.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.upDownProvSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialogConfig;
		private System.Windows.Forms.Button btnSetPath;
		private System.Windows.Forms.TextBox txtEndName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox ckbxnoncontiguous;
		private System.Windows.Forms.CheckBox ckbxExisting;
		private System.Windows.Forms.CheckBox ckbxWarningFile;
		private System.Windows.Forms.NumericUpDown upDownProvRadius;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox grpContiguous;
		private System.Windows.Forms.NumericUpDown upDownProvSize;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnExportConfig;
		private System.Windows.Forms.TextBox txtPath;
		private System.Windows.Forms.CheckBox ckbxDebug;
		private System.Windows.Forms.Button btnImportConfig;
		private System.Windows.Forms.Button btnSubmit;
		private System.Windows.Forms.SaveFileDialog saveFileDialogConfig;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogPath;
	}
}
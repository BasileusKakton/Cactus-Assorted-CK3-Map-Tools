using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Colorer_GUI
{
	public partial class config : Form
	{
		private Form1 mainForm = null;
		private string PATH = "";
		public config(Form callingForm)
		{
			mainForm = callingForm as Form1;
			InitializeComponent();
		}

		private void btnExportConfig_Click(object sender, EventArgs e)
		{

		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			if (!verifyOutputs()) {
				return;
			}
			mainForm.applyConfig(PATH, txtEndName.Text, ckbxWarningFile.Checked, ckbxDebug.Checked, ckbxExisting.Checked, ckbxnoncontiguous.Checked, (int)upDownProvRadius.Value, (int)upDownProvSize.Value);
			this.Close();
		}

		private bool verifyOutputs()
		{
			if (txtPath.Text == "") 
			{
				MessageBox.Show("Warning, Path not set");
				return false;
			}
			if (txtEndName.Text == "")
			{
				MessageBox.Show("Warning, end file name not set");
				return false;
			}

			return true;
		}

		private void btnSetPath_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialogPath.ShowDialog() == DialogResult.OK) //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=net-5.0
			{
				//var fileStream = openFileDialogConfig.OpenFile(); //Read the contents of the file into a stream
				if (File.Exists(folderBrowserDialogPath.SelectedPath + "provinces.png"))
				{
					MessageBox.Show("Warning, no provinces.png in path. Path not set");
				}
				else
				{
					PATH = folderBrowserDialogPath.SelectedPath + "\\";
					txtPath.Text = PATH;
				}
			}
		}

		private void ckbxnoncontiguous_CheckedChanged(object sender, EventArgs e)
		{
			if (!ckbxnoncontiguous.Checked)
			{
				grpContiguous.Enabled = false;
			}
			else
			{
				grpContiguous.Enabled = true;
			}
		}
	}
}

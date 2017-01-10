// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace NDoc.Core.PropertyGridUI
{
	/// <summary>
	/// Used in the conjunction with the <see cref="TextEditor"/>, this form
	/// provides the user a larger interface with which to edit text.
	/// </summary>
	internal class TextEditorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TextBox textBoxEntry;
		private System.Windows.Forms.Label DescriptionLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new instance of the <see cref="TextEditorForm"/> form.
		/// </summary>
		public TextEditorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.textBoxEntry = new System.Windows.Forms.TextBox();
			this.DescriptionLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(288, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(88, 24);
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(192, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(88, 24);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			// 
			// textBoxEntry
			// 
			this.textBoxEntry.AllowDrop = true;
			this.textBoxEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxEntry.Location = new System.Drawing.Point(8, 32);
			this.textBoxEntry.Multiline = true;
			this.textBoxEntry.Name = "textBoxEntry";
			this.textBoxEntry.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxEntry.Size = new System.Drawing.Size(368, 208);
			this.textBoxEntry.TabIndex = 2;
			this.textBoxEntry.Text = "";
			this.textBoxEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxEntry_KeyDown);
			this.textBoxEntry.DragOver += new System.Windows.Forms.DragEventHandler(this.textBoxEntry_DragOver);
			this.textBoxEntry.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxEntry_DragDrop);
			// 
			// DescriptionLabel
			// 
			this.DescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.DescriptionLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.DescriptionLabel.Location = new System.Drawing.Point(8, 8);
			this.DescriptionLabel.Name = "DescriptionLabel";
			this.DescriptionLabel.Size = new System.Drawing.Size(368, 23);
			this.DescriptionLabel.TabIndex = 3;
			this.DescriptionLabel.Text = "You may either edit the text below or drag and drop existing text or a file.";
			// 
			// TextEditorForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(384, 286);
			this.ControlBox = false;
			this.Controls.Add(this.DescriptionLabel);
			this.Controls.Add(this.textBoxEntry);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.MaximizeBox = false;
			this.Name = "TextEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Text Editor";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void textBoxEntry_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text) ||
				e.Data.GetDataPresent(DataFormats.UnicodeText))
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;

			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void textBoxEntry_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			string text = String.Empty;

			if (e.Data.GetDataPresent(DataFormats.Text))
				text = (string)e.Data.GetData(DataFormats.Text);

			else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
				text = (string)e.Data.GetData(DataFormats.UnicodeText);

			else if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length < 1) return;

				// Take the first file and read it in.
				StreamReader reader = null;
				try
				{
					reader = new StreamReader(files[0], System.Text.Encoding.ASCII, true);
					text = reader.ReadToEnd();
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error reading file: " + ex.Message, "NDoc",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				finally
				{
					reader.Close();
				}
			}

			this.Value = text;
		}

		private void textBoxEntry_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.Control && (e.KeyCode == Keys.A))
			{
				textBoxEntry.SelectAll();
				e.Handled = true;
			}
		}

		/// <summary>
		/// Gets or sets the text to edit.
		/// </summary>
		/// <value>The text to edit.</value>
		[Category("Appearance")]
		[Description("The text to edit.")]
		public string Value
		{
			get { return this.textBoxEntry.Text; }
			set { this.textBoxEntry.Text = value; }
		}
	}
}

// AssemblySlashDocForm.cs - form for adding assembly and /doc filename pairs
// Copyright (C) 2001  Kral Ferch
//
// Modified by: Keith Hill on Sep 28, 2001.  
//   Tweaked the layout, made the dialog not show up in the task bar and changed 
//   to title to reflect the terminology used by VS.NET Beta 2 (XML Documentation 
//   vs. /doc).
//
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
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using NDoc.Core;

namespace NDoc.Gui
{

	/// <summary>
	///    This form allows the user to select an assembly and it's matching /doc file.
	/// </summary>
	public class AssemblySlashDocForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private NDoc.Core.PropertyGridUI.RuntimePropertyGrid runtimePropertyGrid1;

		private AssemblySlashDoc assySlashDoc = new AssemblySlashDoc();

		/// <summary>
		/// Gets or sets the AssemblySlashDoc.
		/// </summary>
		/// <value></value>
		public AssemblySlashDoc AssySlashDoc 
		{
			get 
			{ 
				return assySlashDoc;
			}
			set 
			{
				assySlashDoc = value;
				runtimePropertyGrid1.SelectedObject = assySlashDoc;
			}
		} 

		/// <summary>Initializes a new instance of the AssemblySlashDocForm class.</summary>
		public AssemblySlashDocForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			okButton.Enabled = false;
			runtimePropertyGrid1.SelectedObject = assySlashDoc;
		}

		private void AssemblySlashDocForm_Load(object sender, System.EventArgs e)
		{
			UpdateOkButton();
			this.runtimePropertyGrid1.LabelWidth = this.runtimePropertyGrid1.Width / 4;
		}
		
		/// <summary>Clean up any resources being used.</summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.runtimePropertyGrid1 = new NDoc.Core.PropertyGridUI.RuntimePropertyGrid();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(400, 128);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(88, 24);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "OK";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(496, 128);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(88, 24);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			// 
			// runtimePropertyGrid1
			// 
			this.runtimePropertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.runtimePropertyGrid1.CommandsVisibleIfAvailable = true;
			this.runtimePropertyGrid1.HelpVisible = false;
			this.runtimePropertyGrid1.LargeButtons = false;
			this.runtimePropertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.runtimePropertyGrid1.Location = new System.Drawing.Point(8, 8);
			this.runtimePropertyGrid1.Name = "runtimePropertyGrid1";
			this.runtimePropertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.runtimePropertyGrid1.Size = new System.Drawing.Size(576, 112);
			this.runtimePropertyGrid1.TabIndex = 8;
			this.runtimePropertyGrid1.Text = "runtimePropertyGrid1";
			this.runtimePropertyGrid1.ToolbarVisible = false;
			this.runtimePropertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.runtimePropertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.runtimePropertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.runtimePropertyGrid1_PropertyValueChanged);
			// 
			// AssemblySlashDocForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(592, 166);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.runtimePropertyGrid1);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(2048, 300);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(256, 172);
			this.Name = "AssemblySlashDocForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Edit Assembly Filename and XML Documentation Filename";
			this.Load += new System.EventHandler(this.AssemblySlashDocForm_Load);
			this.ResumeLayout(false);

		}


		private void runtimePropertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			UpdateOkButton();
			
			if ((AssySlashDoc.Assembly.Path.Length > 4) && (AssySlashDoc.SlashDoc.Path.Length == 0))
			{
				string slashDocFilename = AssySlashDoc.Assembly.Path.Substring(0, AssySlashDoc.Assembly.Path.Length - 4) + ".xml";
			
				if (File.Exists(slashDocFilename))
				{
					AssySlashDoc.SlashDoc.Path = slashDocFilename;
				}
			}
		}

		private void UpdateOkButton()
		{
			if (AssySlashDoc.Assembly.Path.Length > 4)
			{
				okButton.Enabled = true;
			}
			else
			{
				okButton.Enabled = false;
			}
		}

	}
}

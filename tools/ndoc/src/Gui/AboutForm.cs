// AboutForm.cs - About box form for NDoc GUI interface.
// Copyright (C) 2001  Keith Hill
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
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace NDoc.Gui
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : System.Windows.Forms.Form
	{
		#region Fields
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.RichTextBox richTextBox;
		private GroupBox versionHeaderGroupBox;
		private System.Windows.Forms.ColumnHeader assemblyColumnHeader;
		private System.Windows.Forms.ListView assembliesListView;
		private System.Windows.Forms.ColumnHeader versionColumnHeader;
		private System.Windows.Forms.ColumnHeader dateColumnHeader;
		private System.Windows.Forms.LinkLabel projectHomePageLinkLabel;
		private System.Windows.Forms.LinkLabel adminsLinkLabel;
		private System.Windows.Forms.Label developersLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion // Fields

		#region Constructor / Dispose
		/// <summary>
		/// 
		/// </summary>
		public AboutForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Read RTF file from manifest resource stream and display it in the
			// RichTextBox.  NOTE: Edit the About.RTF with WordPad or Word.
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream stream = assembly.GetManifestResourceStream("NDoc.Gui.About.rtf");
			richTextBox.LoadFile(stream, RichTextBoxStreamType.RichText);

			// Set up web links
			projectHomePageLinkLabel.Links.Add(19, 28, "http://ndoc.sourceforge.net");

			AddLink(adminsLinkLabel,"Kevin Downs","http://sourceforge.net/sendmessage.php?touser=919791");
			adminsLinkLabel.Text += ", ";
			AddLink(adminsLinkLabel,"Don Kackman","http://sourceforge.net/sendmessage.php?touser=4516");
//			adminsLinkLabel.Text += ", ";
//			AddLink(adminsLinkLabel,"Jason Diamond","http://sourceforge.net/sendmessage.php?touser=87620");
//			adminsLinkLabel.Text += ", ";
//			AddLink(adminsLinkLabel,"Jean-Claude Manoli","http://sourceforge.net/sendmessage.php?touser=235364");
//			adminsLinkLabel.Text += ", ";
//			AddLink(adminsLinkLabel,"Kral Ferch","http://sourceforge.net/sendmessage.php?touser=97544");

			
			// Fill in loaded modules / version number info list view.
			try 
			{
				// Get all modules
				ArrayList ndocItems = new ArrayList();
				foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
				{
					ListViewItem item = new ListViewItem();
					item.Text = module.ModuleName;

					// Get version info
					FileVersionInfo verInfo = module.FileVersionInfo;
					string versionStr = String.Format("{0}.{1}.{2}.{3}", 
						                              verInfo.FileMajorPart,
					                                  verInfo.FileMinorPart,
					                                  verInfo.FileBuildPart,
					                                  verInfo.FilePrivatePart);
					item.SubItems.Add(versionStr);

					// Get file date info
					DateTime lastWriteDate = File.GetLastWriteTime(module.FileName);
					string dateStr = lastWriteDate.ToString("g");
					item.SubItems.Add(dateStr);

					assembliesListView.Items.Add(item);

					// Stash ndoc related list view items for later
					if (module.ModuleName.ToLower().StartsWith("ndoc"))
					{
						ndocItems.Add(item);
					}
				}

				// Extract the NDoc related modules and move them to the top
				for (int i = ndocItems.Count; i > 0; i--)
				{
					ListViewItem ndocItem = (ListViewItem)ndocItems[i-1];
					assembliesListView.Items.Remove(ndocItem);
					assembliesListView.Items.Insert(0, ndocItem);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.ToString(), "NDoc Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddLink(LinkLabel ll, string text, string link)
		{
			ll.Links.Add(ll.Text.Length , text.Length, link);
			ll.Text += text;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#endregion // Constructor / Dispose

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.assembliesListView = new System.Windows.Forms.ListView();
			this.assemblyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.versionColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.dateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.closeButton = new System.Windows.Forms.Button();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.versionHeaderGroupBox = new GroupBox();
			this.projectHomePageLinkLabel = new System.Windows.Forms.LinkLabel();
			this.adminsLinkLabel = new System.Windows.Forms.LinkLabel();
			this.developersLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.versionHeaderGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// assembliesListView
			// 
			this.assembliesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.assembliesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								 this.assemblyColumnHeader,
																								 this.versionColumnHeader,
																								 this.dateColumnHeader});
			this.assembliesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.assembliesListView.Location = new System.Drawing.Point(8, 16);
			this.assembliesListView.Name = "assembliesListView";
			this.assembliesListView.Size = new System.Drawing.Size(510, 138);
			this.assembliesListView.TabIndex = 0;
			this.assembliesListView.View = System.Windows.Forms.View.Details;
			// 
			// assemblyColumnHeader
			// 
			this.assemblyColumnHeader.Text = "Module";
			this.assemblyColumnHeader.Width = 208;
			// 
			// versionColumnHeader
			// 
			this.versionColumnHeader.Text = "Version";
			this.versionColumnHeader.Width = 147;
			// 
			// dateColumnHeader
			// 
			this.dateColumnHeader.Text = "Date";
			this.dateColumnHeader.Width = 124;
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.closeButton.Location = new System.Drawing.Point(448, 444);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(88, 24);
			this.closeButton.TabIndex = 0;
			this.closeButton.Text = "&Close";
			// 
			// richTextBox
			// 
			this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox.Location = new System.Drawing.Point(16, 0);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.ReadOnly = true;
			this.richTextBox.Size = new System.Drawing.Size(518, 144);
			this.richTextBox.TabIndex = 4;
			this.richTextBox.Text = "";
			// 
			// versionHeaderGroupBox
			// 
			this.versionHeaderGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.versionHeaderGroupBox.Controls.Add(this.assembliesListView);
			this.versionHeaderGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.versionHeaderGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.versionHeaderGroupBox.Location = new System.Drawing.Point(8, 264);
			this.versionHeaderGroupBox.Name = "versionHeaderGroupBox";
			this.versionHeaderGroupBox.Size = new System.Drawing.Size(526, 168);
			this.versionHeaderGroupBox.TabIndex = 1;
			this.versionHeaderGroupBox.TabStop = false;
			this.versionHeaderGroupBox.Text = "Version Information";
			// 
			// projectHomePageLinkLabel
			// 
			this.projectHomePageLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.projectHomePageLinkLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.projectHomePageLinkLabel.Location = new System.Drawing.Point(16, 152);
			this.projectHomePageLinkLabel.Name = "projectHomePageLinkLabel";
			this.projectHomePageLinkLabel.Size = new System.Drawing.Size(520, 16);
			this.projectHomePageLinkLabel.TabIndex = 5;
			this.projectHomePageLinkLabel.TabStop = true;
			this.projectHomePageLinkLabel.Text = "Project home page: http://ndoc.sourceforge.net";
			this.projectHomePageLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.projectHomePageLinkLabel_LinkClicked);
			// 
			// adminsLinkLabel
			// 
			this.adminsLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.adminsLinkLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.adminsLinkLabel.Location = new System.Drawing.Point(128, 176);
			this.adminsLinkLabel.Name = "adminsLinkLabel";
			this.adminsLinkLabel.Size = new System.Drawing.Size(408, 16);
			this.adminsLinkLabel.TabIndex = 6;
			this.adminsLinkLabel.TabStop = true;
			this.adminsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.adminsLinkLabel_LinkClicked);
			// 
			// developersLabel
			// 
			this.developersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.developersLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.developersLabel.Location = new System.Drawing.Point(88, 200);
			this.developersLabel.Name = "developersLabel";
			this.developersLabel.Size = new System.Drawing.Size(440, 64);
			this.developersLabel.TabIndex = 7;
			this.developersLabel.Text = "Jason Diamond, Jean-Claude Manoli, Kral Ferch, Carlos Guzmán Álvarez, Gert Driese" +
				"n, Heath Stewart, Laurent Domenech, Jerome Mathieu, Keith Hill, Michael Poettgen" +
				", Pascal Bourque, Ryan Seghers, Steve Van Esch, Thong (Tum) Nguyen, Wolfgang Bau" +
				"er";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 176);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "Project administrators : ";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 200);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 40);
			this.label2.TabIndex = 9;
			this.label2.Text = "Contributors : ";
			// 
			// AboutForm
			// 
			this.AcceptButton = this.closeButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(546, 480);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.adminsLinkLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.developersLabel);
			this.Controls.Add(this.projectHomePageLinkLabel);
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.versionHeaderGroupBox);
			this.Controls.Add(this.closeButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.Text = "About NDoc";
			this.versionHeaderGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region event handlers
		private void projectHomePageLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			projectHomePageLinkLabel.Links[projectHomePageLinkLabel.Links.IndexOf(e.Link)].Visited = true;
			string url = e.Link.LinkData.ToString();
			Process.Start(url);
		}

		private void adminsLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			adminsLinkLabel.Links[adminsLinkLabel.Links.IndexOf(e.Link)].Visited = true;
			string url = e.Link.LinkData.ToString();
			Process.Start(url);
		}
		#endregion
	}
}

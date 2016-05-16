// Copyright (C) 2004  Kevin Downs
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
using System.Drawing;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Diagnostics;
using System.Reflection;

using NDoc.ExtendedUI;

namespace NDoc.Core
{
	/// <summary>
	/// Summary description for ReferencePathCollectionEditorForm.
	/// </summary>
	internal class ReferencePathCollectionEditorForm : System.Windows.Forms.Form
	{
		private ListView listView1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnTest;
		private NDoc.Core.PropertyGridUI.RuntimePropertyGrid propertyGrid1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Creates a new <see cref="ReferencePathCollectionEditorForm"/> instance.
		/// </summary>
		public ReferencePathCollectionEditorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// manually add image resources.
			// This avoids problems with VS.NET designer versioning. 
			Assembly assembly = Assembly.GetExecutingAssembly();

			ImageList.ImageCollection imlcol = this.imageList1.Images;

			imlcol.Add(new Bitmap(assembly.GetManifestResourceStream("NDoc.Core.graphics.folder.bmp")));
			imlcol.Add(new Bitmap(assembly.GetManifestResourceStream("NDoc.Core.graphics.folderpin.bmp")));
			imlcol.Add(new Bitmap(assembly.GetManifestResourceStream("NDoc.Core.graphics.multifolder.bmp")));
			imlcol.Add(new Bitmap(assembly.GetManifestResourceStream("NDoc.Core.graphics.multifolderpin.bmp")));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
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

		ReferencePathCollection _refPaths = null;
		/// <summary>
		/// Gets or sets the reference paths collection to edit.
		/// </summary>
		/// <value></value>
		public ReferencePathCollection ReferencePaths
		{
			get { return _refPaths; }
			set
			{
				_refPaths = new ReferencePathCollection();
				foreach (ReferencePath rp in value)
				{
					_refPaths.Add(new ReferencePath(rp));
				}
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ReferencePathCollectionEditorForm));
			this.propertyGrid1 = new NDoc.Core.PropertyGridUI.RuntimePropertyGrid();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.btnTest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(8, 208);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
			this.propertyGrid1.Size = new System.Drawing.Size(456, 136);
			this.propertyGrid1.TabIndex = 5;
			this.propertyGrid1.Text = "propertyGrid";
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(376, 352);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 24);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(280, 352);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(88, 24);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(104, 168);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(88, 24);
			this.btnRemove.TabIndex = 3;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(8, 168);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(88, 24);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1});
			this.listView1.FullRowSelect = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(8, 8);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(456, 152);
			this.listView1.SmallImageList = this.imageList1;
			this.listView1.TabIndex = 1;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "ReferencePath";
			this.columnHeader1.Width = 429;
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnTest
			// 
			this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnTest.Location = new System.Drawing.Point(368, 168);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(96, 24);
			this.btnTest.TabIndex = 4;
			this.btnTest.Text = "Test";
			this.btnTest.Visible = false;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// ReferencePathCollectionEditorForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(472, 390);
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.propertyGrid1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(480, 400);
			this.Name = "ReferencePathCollectionEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ReferencePath Collection Editor";
			this.Load += new System.EventHandler(this.ReferencePathCollectionEditorForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void listView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Insert : 
				{
					this.btnAdd_Click(this.btnAdd, EventArgs.Empty);
					e.Handled = true;
					return;
				}
				case Keys.Delete : 
				{
					this.btnRemove_Click(this.btnRemove, EventArgs.Empty);
					e.Handled = true;
					return;
				}
			}
			if (e.Control && (e.KeyCode == Keys.A))
			{
				foreach (ListViewItem li in listView1.Items)
				{
					li.Selected = true;
				}
				e.Handled = true;
			}
		}


		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			string path = null;

			ShellBrowseForFolderDialog folderDialog = new ShellBrowseForFolderDialog();
			folderDialog.hwndOwner = this.Handle;
			
			if (folderDialog.ShowDialog() == DialogResult.OK) 
			{
				path = folderDialog.FullName;

				foreach (ListViewItem sel_li in listView1.SelectedItems)
				{
					sel_li.Selected = false;
				}
				ReferencePath rp = new ReferencePath();
				rp.Path = path;
				ListViewItem li = new ListViewItem();
				listView1.Items.Add(li);
				li.Tag = rp;
				UpdateListItem(li);
				_refPaths.Add(rp);
				li.Selected = true;
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			foreach (ListViewItem li in listView1.SelectedItems)
			{
				_refPaths.Remove((ReferencePath)li.Tag);
				listView1.Items.Remove(li);
			}
		}

		private void btnTest_Click(object sender, System.EventArgs e)
		{
		
		}

		private void ReferencePathCollectionEditorForm_Load(object sender, System.EventArgs e)
		{
			foreach (ReferencePath rp in _refPaths)
			{
				ListViewItem li = new ListViewItem();
				li.Tag = rp;
				UpdateListItem(li);
				listView1.Items.Add(li);
			}
			if (listView1.Items.Count > 0) listView1.Items[0].Selected = true;
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listView1.SelectedItems.Count == 1)
			{
				propertyGrid1.SelectedObject = new RefPathPropGridProxy(this);
				if (propertyGrid1.SelectedGridItem.Expandable)
					propertyGrid1.SelectedGridItem.Expanded = true;
			}
			else
			{
				object[] si = new object[this.listView1.SelectedItems.Count];
				for (int i = 0; i < listView1.SelectedItems.Count; i++)
				{
					si[i] = listView1.SelectedItems[i].Tag;
				}
				propertyGrid1.SelectedObjects = si;
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			foreach (ListViewItem li in listView1.SelectedItems)
			{
				UpdateListItem(li);
			}
		}

		private void UpdateListItem(ListViewItem li)
		{
			ReferencePath rp = (ReferencePath)li.Tag;
			li.Text = rp.ToString();
			int imageIndex;
			if (rp.FixedPath)
				imageIndex = 1;
			else
				imageIndex = 0;
			if (rp.IncludeSubDirectories)
				imageIndex += 2;
			li.ImageIndex = imageIndex;
			if (rp.Path.Length == 0) li.ForeColor = Color.Black;
		}

		private class RefPathPropGridProxy
		{
			public RefPathPropGridProxy(ReferencePathCollectionEditorForm editorForm)
			{
				_editorForm = editorForm;
				ListViewItem li = editorForm.listView1.SelectedItems[0];
				_listViewItem = li;
				_referencePath = (ReferencePath)li.Tag;
			}

			private ReferencePathCollectionEditorForm _editorForm;
			private ReferencePath _referencePath;
			private ListViewItem  _listViewItem;

			[Editor(typeof(ReferencePath.UIEditor), typeof(UITypeEditor))]
			[NDoc.Core.PropertyGridUI.FoldernameEditor.FolderDialogTitle("Select Reference Path")]
			public ReferencePath ReferencePath 
			{
				get { return _referencePath; }
				set 
				{
					_referencePath = value;
					if(!Object.ReferenceEquals(_listViewItem.Tag,_referencePath))
					{
						_listViewItem.Tag=_referencePath;
						_editorForm._refPaths[_listViewItem.Index]=_referencePath;
					}
				}
			} 
		}
	}
}

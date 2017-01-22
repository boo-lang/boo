// AssemblyListControl.cs 
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NDoc.Core;

namespace NDoc.Gui
{
	/// <summary>
	/// Control that displays the Assemblies in a project
	/// </summary>
	public class AssemblyListControl : UserControl
	{
		private System.Windows.Forms.ListView assembliesListView;
		private System.Windows.Forms.Button editButton;
		private System.Windows.Forms.Button namespaceSummariesButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.ColumnHeader nameColumnHeader;
		private System.Windows.Forms.ColumnHeader pathColumnHeader;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.ContextMenu itemContextMenu1;
		private System.Windows.Forms.MenuItem editMenuItem1;
		private System.Windows.Forms.MenuItem removeMenuItem2;
		private System.Windows.Forms.ContextMenu blankSpaceContextMenu;
		private System.Windows.Forms.MenuItem detailsMenuItem1;
		private System.Windows.Forms.MenuItem listMenuItem2;
		private System.Windows.Forms.MenuItem exploreMenuItem1;
		private System.Windows.Forms.ColumnHeader fixedColumnHeader1;
		private System.Windows.Forms.MenuItem propertiesMenuItem1;
		private System.Windows.Forms.MenuItem menuItem1;
		private AssemblySlashDocCollection _AssemblySlashDocs; 
	
		/// <summary>
		/// Creates an isntance of the class
		/// </summary>
		public AssemblyListControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets the collection of assemblies and documentation comment XML files in the project.
		/// </summary>
		/// <value>An <see cref="AssemblySlashDocCollection"/>.</value>
		public AssemblySlashDocCollection AssemblySlashDocs 
		{
			get 
			{ 
				return _AssemblySlashDocs; 
			}
			set
			{
				this.assembliesListView.Items.Clear();

				if ( _AssemblySlashDocs != null )
				{
					_AssemblySlashDocs.ItemAdded -= new AssemblySlashDocEventHandler(_AssemblySlashDocs_ItemAdded);
					_AssemblySlashDocs.ItemRemoved -= new AssemblySlashDocEventHandler(_AssemblySlashDocs_ItemRemoved);
					_AssemblySlashDocs.Cleared -= new EventHandler(_AssemblySlashDocs_Cleared);
				}

				_AssemblySlashDocs = value;

				if ( _AssemblySlashDocs != null )
				{
					_AssemblySlashDocs.ItemAdded += new AssemblySlashDocEventHandler(_AssemblySlashDocs_ItemAdded);
					_AssemblySlashDocs.ItemRemoved += new AssemblySlashDocEventHandler(_AssemblySlashDocs_ItemRemoved);
					_AssemblySlashDocs.Cleared += new EventHandler(_AssemblySlashDocs_Cleared);

					this.assembliesListView.BeginUpdate();

					foreach ( AssemblySlashDoc assemblySlashDoc in _AssemblySlashDocs )
						AddListViewItem( assemblySlashDoc );

					this.assembliesListView.EndUpdate();
				}
			}
		} 

		/// <summary>
		/// See <see cref="Control.Refresh"/>
		/// </summary>
		public override void Refresh()
		{
			foreach ( ListViewItem item in this.assembliesListView.Items )
			{
				AssemblySlashDoc assemblySlashDoc = item.Tag as AssemblySlashDoc;
				Debug.Assert( assemblySlashDoc != null );

				if ( File.Exists( assemblySlashDoc.Assembly.Path ) )
					item.ForeColor = Color.Black;
				else
					item.ForeColor = Color.Red;
			}

			base.Refresh ();
		}

		private void AddListViewItem( AssemblySlashDoc assemblySlashDoc )
		{
			Debug.Assert( assemblySlashDoc != null );

			ListViewItem item = new ListViewItem( Path.GetFileName( assemblySlashDoc.Assembly.Path ) );
			item.SubItems.Add( Path.GetDirectoryName( assemblySlashDoc.Assembly.Path ) );
			item.SubItems.Add( GetFixedPathType( assemblySlashDoc ) );
			item.Tag = assemblySlashDoc;

			if ( File.Exists( assemblySlashDoc.Assembly.Path ) == false )
				item.ForeColor = Color.Red;

			assembliesListView.Items.Add( item );
			item.EnsureVisible();
		}

		#region InitializComponent
		private void InitializeComponent()
		{
			this.assembliesListView = new System.Windows.Forms.ListView();
			this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.pathColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.fixedColumnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.itemContextMenu1 = new System.Windows.Forms.ContextMenu();
			this.editMenuItem1 = new System.Windows.Forms.MenuItem();
			this.removeMenuItem2 = new System.Windows.Forms.MenuItem();
			this.exploreMenuItem1 = new System.Windows.Forms.MenuItem();
			this.propertiesMenuItem1 = new System.Windows.Forms.MenuItem();
			this.editButton = new System.Windows.Forms.Button();
			this.namespaceSummariesButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.blankSpaceContextMenu = new System.Windows.Forms.ContextMenu();
			this.detailsMenuItem1 = new System.Windows.Forms.MenuItem();
			this.listMenuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// assembliesListView
			// 
			this.assembliesListView.AllowDrop = true;
			this.assembliesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.assembliesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								 this.nameColumnHeader,
																								 this.pathColumnHeader,
																								 this.fixedColumnHeader1});
			this.assembliesListView.ContextMenu = this.itemContextMenu1;
			this.assembliesListView.ForeColor = System.Drawing.SystemColors.WindowText;
			this.assembliesListView.FullRowSelect = true;
			this.assembliesListView.GridLines = true;
			this.assembliesListView.HideSelection = false;
			this.assembliesListView.Location = new System.Drawing.Point(0, 0);
			this.assembliesListView.Name = "assembliesListView";
			this.assembliesListView.Size = new System.Drawing.Size(368, 136);
			this.assembliesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.assembliesListView.TabIndex = 18;
			this.assembliesListView.View = System.Windows.Forms.View.Details;
			this.assembliesListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.assembliesListView_MouseDown);
			this.assembliesListView.ItemActivate += new System.EventHandler(this.assembliesListView_ItemActivate);
			this.assembliesListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.assembliesListView_DragDrop);
			this.assembliesListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.assembliesListView_DragEnter);
			this.assembliesListView.SelectedIndexChanged += new System.EventHandler(this.assembliesListView_SelectedIndexChanged);
			// 
			// nameColumnHeader
			// 
			this.nameColumnHeader.Text = "Assembly";
			this.nameColumnHeader.Width = 96;
			// 
			// pathColumnHeader
			// 
			this.pathColumnHeader.Text = "Path";
			this.pathColumnHeader.Width = 236;
			// 
			// fixedColumnHeader1
			// 
			this.fixedColumnHeader1.Text = "Fixed";
			this.fixedColumnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// itemContextMenu1
			// 
			this.itemContextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.editMenuItem1,
																							 this.removeMenuItem2,
																							 this.menuItem1,
																							 this.exploreMenuItem1,
																							 this.propertiesMenuItem1});
			// 
			// editMenuItem1
			// 
			this.editMenuItem1.DefaultItem = true;
			this.editMenuItem1.Index = 0;
			this.editMenuItem1.Text = "Edit...";
			this.editMenuItem1.Click += new System.EventHandler(this.editMenuItem1_Click);
			// 
			// removeMenuItem2
			// 
			this.removeMenuItem2.Index = 1;
			this.removeMenuItem2.Text = "Remove";
			this.removeMenuItem2.Click += new System.EventHandler(this.removeMenuItem2_Click);
			// 
			// exploreMenuItem1
			// 
			this.exploreMenuItem1.Index = 3;
			this.exploreMenuItem1.Text = "Find...";
			this.exploreMenuItem1.Click += new System.EventHandler(this.exploreMenuItem1_Click);
			// 
			// propertiesMenuItem1
			// 
			this.propertiesMenuItem1.Index = 4;
			this.propertiesMenuItem1.Text = "Properties...";
			this.propertiesMenuItem1.Click += new System.EventHandler(this.propertiesMenuItem1_Click);
			// 
			// editButton
			// 
			this.editButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.editButton.Enabled = false;
			this.editButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.editButton.Location = new System.Drawing.Point(376, 36);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(88, 24);
			this.editButton.TabIndex = 20;
			this.editButton.Text = "Edit";
			this.editButton.Click += new System.EventHandler(this.editButton_Click);
			// 
			// namespaceSummariesButton
			// 
			this.namespaceSummariesButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.namespaceSummariesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.namespaceSummariesButton.Location = new System.Drawing.Point(376, 100);
			this.namespaceSummariesButton.Name = "namespaceSummariesButton";
			this.namespaceSummariesButton.Size = new System.Drawing.Size(88, 32);
			this.namespaceSummariesButton.TabIndex = 22;
			this.namespaceSummariesButton.Text = "Namespace\nSummaries";
			this.namespaceSummariesButton.Click += new System.EventHandler(this.namespaceSummariesButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.deleteButton.Enabled = false;
			this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.deleteButton.Location = new System.Drawing.Point(376, 68);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(88, 24);
			this.deleteButton.TabIndex = 21;
			this.deleteButton.Text = "Remove";
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// addButton
			// 
			this.addButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.addButton.Location = new System.Drawing.Point(376, 4);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(88, 24);
			this.addButton.TabIndex = 19;
			this.addButton.Text = "Add";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// blankSpaceContextMenu
			// 
			this.blankSpaceContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								  this.detailsMenuItem1,
																								  this.listMenuItem2});
			// 
			// detailsMenuItem1
			// 
			this.detailsMenuItem1.Index = 0;
			this.detailsMenuItem1.Text = "Details";
			this.detailsMenuItem1.Click += new System.EventHandler(this.detailsMenuItem1_Click);
			// 
			// listMenuItem2
			// 
			this.listMenuItem2.Index = 1;
			this.listMenuItem2.Text = "List";
			this.listMenuItem2.Click += new System.EventHandler(this.listMenuItem2_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// AssemblyListControl
			// 
			this.Controls.Add(this.assembliesListView);
			this.Controls.Add(this.editButton);
			this.Controls.Add(this.namespaceSummariesButton);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.addButton);
			this.Name = "AssemblyListControl";
			this.Size = new System.Drawing.Size(464, 136);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void assembliesListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			editButton.Enabled = assembliesListView.SelectedIndices.Count > 0;
			deleteButton.Enabled = assembliesListView.SelectedIndices.Count > 0;
		}

		private void addButton_Click (object sender, System.EventArgs e)
		{
			using ( AssemblySlashDocForm  form = new AssemblySlashDocForm() )
			{
				form.Text = "Add Assembly Filename and XML Documentation Filename";
				form.StartPosition = FormStartPosition.CenterParent;

				if ( form.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
				{
					if ( this._AssemblySlashDocs.Contains( form.AssySlashDoc ) )
						MessageBox.Show( this, "The selected assembly already exists in this project", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
					else
						_AssemblySlashDocs.Add( form.AssySlashDoc );
				}
			}
		}

		private void editButton_Click (object sender, System.EventArgs e)
		{
			if ( assembliesListView.SelectedItems.Count > 0 )
			{
				using ( AssemblySlashDocForm form = new AssemblySlashDocForm() )
				{
					form.Text = "Edit Assembly Filename and XML Documentation Filename";
					form.StartPosition = FormStartPosition.CenterParent;

					ListViewItem item = assembliesListView.SelectedItems[0];
					form.AssySlashDoc = ((AssemblySlashDoc)item.Tag).Clone() as AssemblySlashDoc;

					if ( form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK )
					{
						this.assembliesListView.BeginUpdate();

						this._AssemblySlashDocs.Remove( (AssemblySlashDoc)item.Tag );
						this._AssemblySlashDocs.Add( form.AssySlashDoc );

						this.assembliesListView.EndUpdate();
					}
				}
				this.Refresh();
			}
		}

		private static string GetFixedPathType( AssemblySlashDoc assemblySlashDoc )
		{
			if ( assemblySlashDoc.Assembly.FixedPath == assemblySlashDoc.SlashDoc.FixedPath )
				return assemblySlashDoc.Assembly.FixedPath.ToString();
			else
				return string.Format( "{0}/{1}", assemblySlashDoc.Assembly.FixedPath, assemblySlashDoc.SlashDoc.FixedPath );
		}

		/// <summary>
		/// Removes the selected assembly and /doc file pair from the listview.
		/// </summary>
		/// <remarks>
		/// If the row being deleted was the only one left in the listview then
		/// the documentation buttons are disabled.
		/// </remarks>
		/// <param name="sender">The sender (not used).</param>
		/// <param name="e">The event arguments (not used).</param>
		private void deleteButton_Click (object sender, System.EventArgs e)
		{
			this.assembliesListView.BeginUpdate();
			foreach(ListViewItem listViewItem in assembliesListView.SelectedItems)
				_AssemblySlashDocs.Remove( listViewItem.Tag as AssemblySlashDoc );

			if ( this.assembliesListView.Items.Count > 0 )
			{
				this.assembliesListView.Items[0].Selected = true;
				this.assembliesListView.Items[0].EnsureVisible();
			}
			this.assembliesListView.EndUpdate();
		}

		/// <summary>
		/// Event raised when the "Edit Namespace" button is clicked
		/// </summary>
		public event EventHandler EditNamespaces;
		/// <summary>
		/// Raises the <see cref="EditNamespaces"/> event
		/// </summary>
		protected virtual void OnEditNamespaces()
		{
			if ( EditNamespaces != null )
				EditNamespaces( this, EventArgs.Empty );
		}

		/// <summary>
		/// Brings up the form for entering namespace summaries.
		/// </summary>
		/// <remarks>
		/// Calls XmlDocumenter to build an XML file documenting the assemblies
		/// currently in the project.  This file is used to discover all of the
		/// namespaces currently being documented in case any new ones have been
		/// added.  A <see cref="System.Collections.Hashtable"/> with the namespace
		/// names as keys and any existing summaries as values is passed in to
		/// a form which allows editing of the namespace summaries.  If the ok button
		/// is selected in the form then the Hashtable becomes the main one used by
		/// NDoc and passed into documenters for building documentation.
		/// </remarks>
		/// <param name="sender">The sender (not used).</param>
		/// <param name="e">The event arguments (not used).</param>
		private void namespaceSummariesButton_Click (object sender, System.EventArgs e)
		{
			OnEditNamespaces();
		}

		private void assembliesListView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			this.assembliesListView.BeginUpdate();
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach ( AssemblySlashDoc assemblySlashDoc in DragDropHandler.GetAssemblySlashDocs( files ) )
			{			
				if ( _AssemblySlashDocs.Contains( assemblySlashDoc ) == false )
					_AssemblySlashDocs.Add( assemblySlashDoc );
			}
			this.assembliesListView.EndUpdate();
		}

		private void assembliesListView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if( e.Data.GetDataPresent(DataFormats.FileDrop) && DragDropHandler.CanDrop( (string[])e.Data.GetData( DataFormats.FileDrop ) ) == DropFileType.Assembly )
				e.Effect = DragDropEffects.Link;

			else
				e.Effect = DragDropEffects.None;
		}

		private void assembliesListView_ItemActivate(object sender, System.EventArgs e)
		{
			editButton_Click(sender, e);		
		}

		private void _AssemblySlashDocs_ItemAdded(object sender, AssemblySlashDocEventArgs args)
		{
			AddListViewItem( args.AssemblySlashDoc );
		}

		private void _AssemblySlashDocs_ItemRemoved(object sender, AssemblySlashDocEventArgs args)
		{
			foreach ( ListViewItem item in this.assembliesListView.Items )
			{
				if ( item.Tag == args.AssemblySlashDoc )
				{
					item.Remove();
					break;
				}
			}
		}

		private void _AssemblySlashDocs_Cleared(object sender, EventArgs e)
		{
			this.assembliesListView.Items.Clear();
		}

		private void assembliesListView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
			{
				ListViewItem item = assembliesListView.GetItemAt( e.X, e.Y );

				if ( item != null )
					assembliesListView.ContextMenu = this.itemContextMenu1;
				else
					assembliesListView.ContextMenu = this.blankSpaceContextMenu;
			}
		}

		private void editMenuItem1_Click(object sender, System.EventArgs e)
		{
			editButton_Click( sender, e );
		}

		private void removeMenuItem2_Click(object sender, System.EventArgs e)
		{
			deleteButton_Click( sender, e );
		}

		/// <summary>
		/// Event raised when the <see cref="DetailsView"/> property changes
		/// </summary>
		public event EventHandler DetailsViewChanged;
		/// <summary>
		/// Raises the <see cref="DetailsViewChanged"/> event
		/// </summary>
		protected virtual void OnDetailsViewChanged()
		{
			if ( DetailsViewChanged != null )
				DetailsViewChanged( this, EventArgs.Empty );
		}

		/// <summary>
		/// Determines if the ListView is in report or list mode
		/// </summary>
		public bool DetailsView
		{
			get
			{
				return this.assembliesListView.View == View.Details;
			}
			set
			{
				if ( value )
					this.assembliesListView.View = View.Details;
				else
					this.assembliesListView.View = View.List;

				this.detailsMenuItem1.Enabled = !value;
				this.listMenuItem2.Enabled = value;

				OnDetailsViewChanged();
			}
		}

		private void detailsMenuItem1_Click(object sender, System.EventArgs e)
		{
			DetailsView = true;
		}

		private void listMenuItem2_Click(object sender, System.EventArgs e)
		{
			DetailsView = false;
		}

		private void exploreMenuItem1_Click(object sender, System.EventArgs e)
		{
			if ( this.assembliesListView.SelectedIndices.Count > 0 )
			{
				string path = ((AssemblySlashDoc)this.assembliesListView.SelectedItems[0].Tag).Assembly.Path;
				if ( File.Exists( path ) )
					Process.Start( Path.GetDirectoryName( Path.GetFullPath( path ) ) );
				
				else
					MessageBox.Show( this, string.Format( "Could not find the file:\n{0}", path ), "File not found", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
		}

		[DllImport("shell32.dll")]
		private static extern bool SHObjectProperties( IntPtr hwnd, int dwType, IntPtr szObject, IntPtr szPage );
		private const int SHOP_FILEPATH = 2;

		private void propertiesMenuItem1_Click(object sender, System.EventArgs e)
		{
			if ( this.assembliesListView.SelectedIndices.Count > 0 )
			{
				string path = ((AssemblySlashDoc)this.assembliesListView.SelectedItems[0].Tag).Assembly.Path;
				if ( File.Exists( path ) )
				{
					IntPtr szObject = Marshal.StringToHGlobalUni( path );
					if ( szObject != IntPtr.Zero )
					{
						SHObjectProperties( this.Handle, SHOP_FILEPATH, szObject, IntPtr.Zero );
						Marshal.FreeHGlobal( szObject );
					}
					else
					{
						string msg = string.Format( "Could not find the file:\n{0}", path );
						MessageBox.Show( this, msg, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Information );
					}
				}
			}
		}
	}
}
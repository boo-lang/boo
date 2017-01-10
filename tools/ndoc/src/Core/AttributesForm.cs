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

namespace NDoc.Core.PropertyGridUI
{
	/// <summary>
	/// A form to build attributes filter criteria.
	/// </summary>
	internal class AttributesForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Contains the updated value, if the user clicked OK.
		/// </summary>
		public string Value;


		/// <summary>
		/// Structure which holds all the attributes
		/// </summary>
		ArrayList AttributesToShow = new ArrayList();


		private System.Windows.Forms.ListBox listAttributes;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button Add;
		private System.Windows.Forms.Button Delete;
		private System.Windows.Forms.Button Edit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listProperties;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button AddProp;
		private System.Windows.Forms.Button EditProp;
		private System.Windows.Forms.Button DeleteProp;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Creates and initialize a new AttributesForm object.
		/// </summary>
		/// <param name="val"></param>
		public AttributesForm(object val)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Edit.Enabled = false;
			this.Delete.Enabled = false;
			this.EditProp.Enabled = false;
			this.DeleteProp.Enabled = false;
			this.AddProp.Enabled = false;

			if (val == null || val.ToString().Length == 0)
				return;

			string nonparsedval = val.ToString();
			string[] tmparray = new string[200];
			char[] attributeDelimiters = { '|' };
			char[] propertyDelimiters = { ',' };
			tmparray = nonparsedval.Split(attributeDelimiters, 199);

			if (tmparray != null)
			{
				int i,j;

				for(i = 0; i < tmparray.Length; i++)
				{
					string[] tmparray2 = new String[200];
					tmparray2 = tmparray[i].Split(propertyDelimiters, 199);
				
					AttributeToShow attributeToShow = new AttributeToShow();
					attributeToShow.Name = tmparray2[0];

					for(j = 1; j < tmparray2.Length; j++)
					{
						attributeToShow.PropertiesToShow.Add(tmparray2[j]);
					}
					AttributesToShow.Add(attributeToShow);
					this.listAttributes.DataSource = AttributesToShow;
					this.listAttributes.DisplayMember = "Name";
				}
			}
			UpdateAttributes();
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
			this.listAttributes = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.Add = new System.Windows.Forms.Button();
			this.Delete = new System.Windows.Forms.Button();
			this.Edit = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.DeleteProp = new System.Windows.Forms.Button();
			this.EditProp = new System.Windows.Forms.Button();
			this.AddProp = new System.Windows.Forms.Button();
			this.listProperties = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// listAttributes
			// 
			this.listAttributes.Location = new System.Drawing.Point(16, 48);
			this.listAttributes.Name = "listAttributes";
			this.listAttributes.Size = new System.Drawing.Size(296, 251);
			this.listAttributes.TabIndex = 0;
			this.listAttributes.SelectedIndexChanged += new System.EventHandler(this.listAttributes_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(608, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Show only the attributes which start with:    (An empty list will show all the at" +
				"tributes)";
			// 
			// Add
			// 
			this.Add.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Add.Location = new System.Drawing.Point(16, 304);
			this.Add.Name = "Add";
			this.Add.Size = new System.Drawing.Size(88, 24);
			this.Add.TabIndex = 2;
			this.Add.Text = "Add";
			this.Add.Click += new System.EventHandler(this.Add_Click);
			// 
			// Delete
			// 
			this.Delete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Delete.Location = new System.Drawing.Point(208, 304);
			this.Delete.Name = "Delete";
			this.Delete.Size = new System.Drawing.Size(88, 24);
			this.Delete.TabIndex = 3;
			this.Delete.Text = "Delete";
			this.Delete.Click += new System.EventHandler(this.Delete_Click);
			// 
			// Edit
			// 
			this.Edit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Edit.Location = new System.Drawing.Point(112, 304);
			this.Edit.Name = "Edit";
			this.Edit.Size = new System.Drawing.Size(88, 24);
			this.Edit.TabIndex = 4;
			this.Edit.Text = "Edit";
			this.Edit.Click += new System.EventHandler(this.Edit_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.DeleteProp);
			this.groupBox1.Controls.Add(this.EditProp);
			this.groupBox1.Controls.Add(this.AddProp);
			this.groupBox1.Controls.Add(this.listProperties);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(320, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(312, 304);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Properties";
			// 
			// DeleteProp
			// 
			this.DeleteProp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.DeleteProp.Location = new System.Drawing.Point(208, 272);
			this.DeleteProp.Name = "DeleteProp";
			this.DeleteProp.Size = new System.Drawing.Size(88, 24);
			this.DeleteProp.TabIndex = 4;
			this.DeleteProp.Text = "Delete";
			this.DeleteProp.Click += new System.EventHandler(this.DeleteProp_Click);
			// 
			// EditProp
			// 
			this.EditProp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.EditProp.Location = new System.Drawing.Point(112, 272);
			this.EditProp.Name = "EditProp";
			this.EditProp.Size = new System.Drawing.Size(88, 24);
			this.EditProp.TabIndex = 3;
			this.EditProp.Text = "Edit";
			this.EditProp.Click += new System.EventHandler(this.EditProp_Click);
			// 
			// AddProp
			// 
			this.AddProp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AddProp.Location = new System.Drawing.Point(16, 272);
			this.AddProp.Name = "AddProp";
			this.AddProp.Size = new System.Drawing.Size(88, 24);
			this.AddProp.TabIndex = 2;
			this.AddProp.Text = "Add";
			this.AddProp.Click += new System.EventHandler(this.AddProp_Click);
			// 
			// listProperties
			// 
			this.listProperties.Location = new System.Drawing.Point(8, 48);
			this.listProperties.Name = "listProperties";
			this.listProperties.Size = new System.Drawing.Size(296, 212);
			this.listProperties.TabIndex = 1;
			this.listProperties.SelectedIndexChanged += new System.EventHandler(this.listProperties_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(288, 32);
			this.label2.TabIndex = 0;
			this.label2.Text = "Only the following properties of the selected attribute will be shown:  (An empty" +
				" list will show all the properties)";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(432, 368);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(88, 24);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(528, 368);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(88, 24);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.Delete);
			this.groupBox2.Controls.Add(this.Add);
			this.groupBox2.Controls.Add(this.Edit);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(8, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(632, 344);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Attributes";
			// 
			// AttributesForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(650, 408);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listAttributes);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "AttributesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Attributes";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion





		/// <summary>
		/// Helper function which updates the attributes list
		/// </summary>
		private void UpdateAttributes()
		{
			this.listAttributes.DataSource = null;
			this.listAttributes.DataSource = AttributesToShow;
			this.listAttributes.DisplayMember = "Name";

			if (this.listAttributes.SelectedIndex >= 0)
			{
				this.Edit.Enabled = true;
				this.Delete.Enabled = true;
			}
			else
			{
				this.Edit.Enabled = false;
				this.Delete.Enabled = false;
			}

			UpdateProperties();
		}

		/// <summary>
		/// Helper function which udpates the properties list
		/// </summary>
		private void UpdateProperties()
		{
			if (this.listAttributes.SelectedIndex >= 0)
			{
				this.AddProp.Enabled = true;

				if (this.listAttributes.SelectedItem != null)
				{
					this.listProperties.DataSource = null;
					this.listProperties.DataSource = ((AttributeToShow)this.listAttributes.SelectedItem).PropertiesToShow;
				}
				else
				{
					this.listProperties.DataSource = null;
				}
				if (this.listProperties.SelectedIndex >= 0)
				{
					this.EditProp.Enabled = true;
					this.DeleteProp.Enabled = true;
				}
				else
				{
					this.EditProp.Enabled = false;
					this.DeleteProp.Enabled = false;
				}
			}
			else
			{
				this.AddProp.Enabled = false;
			}
		}

		/// <summary>
		/// Event handler called when a new attribute gets selected.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void listAttributes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.listAttributes.SelectedIndex >= 0)
			{
				UpdateProperties();
			}
		}

		/// <summary>
		/// Event handler called when the ADD button is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void Add_Click(object sender, System.EventArgs e)
		{
			SimpleEdit dlg = new SimpleEdit();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				AttributeToShow att = new AttributeToShow();
				att.Name = dlg.Value;
				this.AttributesToShow.Add(att);
				UpdateAttributes();
			}
		}

		/// <summary>
		/// Event handler called when the DELETE button is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void Delete_Click(object sender, System.EventArgs e)
		{
			int index = this.listAttributes.SelectedIndex;
			if (index >= 0)
			{
				if (index >= 1)
				{
					this.listAttributes.SelectedIndex = index -1;
				}
				this.AttributesToShow.RemoveAt(index);
				UpdateAttributes();
			}
		}

		/// <summary>
		/// Event handler called when the EDIT button is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void Edit_Click(object sender, System.EventArgs e)
		{
			int index = this.listAttributes.SelectedIndex;
			if (index >= 0)
			{
				SimpleEdit dlg = new SimpleEdit();
				dlg.Value = ((AttributeToShow)(AttributesToShow[index])).Name;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					AttributeToShow att = new AttributeToShow();
					att.Name = dlg.Value;
					att.PropertiesToShow = (ArrayList)(((AttributeToShow)this.AttributesToShow[index]).PropertiesToShow.Clone());
					this.AttributesToShow.RemoveAt(index);
					AttributesToShow.Insert(index, att);
					UpdateAttributes();
				}
			}
		}

		/// <summary>
		/// Event handler called when the ADD (of the property list) is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void AddProp_Click(object sender, System.EventArgs e)
		{
			SimpleEdit dlg = new SimpleEdit();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				int index = this.listAttributes.SelectedIndex;
				((AttributeToShow)(AttributesToShow[index])).PropertiesToShow.Add(dlg.Value);
				UpdateAttributes();
			}
		}

		/// <summary>
		/// Event handler called when the EDIT (of the property list) is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void EditProp_Click(object sender, System.EventArgs e)
		{
			int index = this.listAttributes.SelectedIndex;
			if (index >= 0)
			{
				int indexProp = this.listProperties.SelectedIndex;
				if (indexProp >= 0)
				{
					SimpleEdit dlg = new SimpleEdit();
					dlg.Value = (string) ((AttributeToShow)(AttributesToShow[index])).PropertiesToShow[indexProp];
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						AttributeToShow att = new AttributeToShow();
						att.Name = ((AttributeToShow)this.AttributesToShow[index]).Name;
						att.PropertiesToShow = (ArrayList)(((AttributeToShow)this.AttributesToShow[index]).PropertiesToShow.Clone());
						att.PropertiesToShow[indexProp] = dlg.Value;
						this.AttributesToShow.RemoveAt(index);
						AttributesToShow.Insert(index, att);
						UpdateAttributes();
					}
				}
			}
		}

		/// <summary>
		/// Event handler called when the DELETE (of the property list) is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void DeleteProp_Click(object sender, System.EventArgs e)
		{
			int index = this.listProperties.SelectedIndex;
			if (index >= 0)
			{
				if (index >= 1)
				{
					this.listProperties.SelectedIndex = index -1;
				}
				((AttributeToShow)(AttributesToShow[listAttributes.SelectedIndex])).PropertiesToShow.RemoveAt(index);
				UpdateAttributes();
				this.listProperties.SelectedIndex = index -1;
			}
		}

		/// <summary>
		/// Event handler called when a new item in the property list gets selected.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void listProperties_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.listProperties.SelectedIndex >= 0)
			{
				this.EditProp.Enabled = true;
				this.DeleteProp.Enabled = true;
			}
			else
			{
				this.EditProp.Enabled = false;
				this.DeleteProp.Enabled = false;
			}
		}

		/// <summary>
		/// Event handler called when the OK button is clicked.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument</param>
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			// Build up the updated value and store in this.Value

			string whole = "";
			for(int i = 0; i < this.AttributesToShow.Count; i++)
			{
				string attributeAndProperty;
				AttributeToShow att = (AttributeToShow)this.AttributesToShow[i];
				attributeAndProperty = att.Name;
				for(int j = 0; j < att.PropertiesToShow.Count; j++)
				{
					attributeAndProperty += ",";
					attributeAndProperty += (string)att.PropertiesToShow[j];
				}

				whole += attributeAndProperty;

				if(i+1 < this.AttributesToShow.Count)
				{
					whole += "|";
				}
			}
			this.Value = whole;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// Class which holds information about an attribute.
		/// </summary>
		private class AttributeToShow
		{
			/// <summary>
			/// Creates an empty AttributeToShow object
			/// </summary>
			public AttributeToShow()
			{
			}

			private string _Name;

			/// <summary>
			/// Name attribute
			/// </summary>
			public string Name
			{
				get 
				{
					return _Name;
				}
				set 
				{
					_Name = value;
				}
			}

			/// <summary>
			/// List of properties
			/// </summary>
			public ArrayList PropertiesToShow = new ArrayList();
		}
	}
}

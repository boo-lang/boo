using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NDoc.Gui
{
	/// <summary>
	/// Form to view and set applicatio options
	/// </summary>
	public class OptionsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button Ok;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private NDocOptions _options;

		/// <summary>
		/// Creates a new instance of the OptionsForm class
		/// </summary>
		public OptionsForm() : this( new NDocOptions() )
		{
		}

		/// <summary>
		/// Creates a new instance of the OptionsForm class
		/// </summary>
		/// <param name="o">An options object to display</param>
		public OptionsForm( NDocOptions o )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_options = o;
			propertyGrid.SelectedObject = _options;
		}

		/// <summary>
		/// Returns the Options
		/// </summary>
		public NDocOptions Options
		{
			get{ return _options; }
		}

		#region Disposer
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
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Ok = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// Ok
			// 
			this.Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Ok.Enabled = false;
			this.Ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Ok.Location = new System.Drawing.Point(182, 310);
			this.Ok.Name = "Ok";
			this.Ok.TabIndex = 0;
			this.Ok.Text = "Ok";
			this.Ok.Click += new System.EventHandler(this.Ok_Click);
			// 
			// Cancel
			// 
			this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Cancel.Location = new System.Drawing.Point(262, 310);
			this.Cancel.Name = "Cancel";
			this.Cancel.TabIndex = 1;
			this.Cancel.Text = "Cancel";
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(8, 16);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.propertyGrid.Size = new System.Drawing.Size(328, 280);
			this.propertyGrid.TabIndex = 2;
			this.propertyGrid.Text = "PropertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// OptionsForm
			// 
			this.AcceptButton = this.Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(346, 344);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.Ok);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "OptionsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NDoc Options";
			this.ResumeLayout(false);

		}
		#endregion

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void Ok_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			this.Ok.Enabled = true;
		}
	}
}

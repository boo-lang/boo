using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NDoc.Gui
{
	/// <summary>
	/// TraceWindow is a class that will connect to trace events and display trace outputs in a text box
	/// </summary>
	public class TraceWindowControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Label captionLabel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem clearMenuItem;
		private System.Windows.Forms.MenuItem copyAllMenuItem;
		private System.Windows.Forms.MenuItem copySelectionMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Creates a new instance of the TraceWindowControl class
		/// </summary>
		public TraceWindowControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Gets/Set the window caption
		/// </summary>
		[Category("Appearance")]
		[Browsable(true)]
		public override string Text
		{
			get{ return captionLabel.Text; }
			set{ captionLabel.Text = value; }
		}

		/// <summary>
		/// Gets/Sets the test displayed in the trace window
		/// </summary>
		public string TraceText
		{
			get{ return richTextBox1.Text; }
			set{ richTextBox1.Text = value; }
		}

		private bool _AutoConnect = false;

		/// <summary>
		/// Determines whether the control will connect to trace events when it becomes visible, and disconnect when it is hidden
		/// </summary>
		[Category("Behavior")]
		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the control will connect to trace events when it becomes visible, and disconnect when it is hidden")]
		public bool AutoConnect
		{
			get{ return _AutoConnect; }
			set{ _AutoConnect = value; }
		}

		/// <summary>
		/// Clears the contents of the window
		/// </summary>
		public void Clear()
		{
			this.richTextBox1.Clear();
		}

		#region Disposer
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				Disconnect();

				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.captionLabel = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.clearMenuItem = new System.Windows.Forms.MenuItem();
			this.copyAllMenuItem = new System.Windows.Forms.MenuItem();
			this.copySelectionMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox1.AutoWordSelection = true;
			this.richTextBox1.CausesValidation = false;
			this.richTextBox1.ContextMenu = this.contextMenu1;
			this.richTextBox1.DetectUrls = false;
			this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox1.HideSelection = false;
			this.richTextBox1.Location = new System.Drawing.Point(0, 16);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(328, 224);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.richTextBox1.WordWrap = false;
			this.richTextBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseDown);
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.BackColor = System.Drawing.SystemColors.Control;
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.closeButton.Location = new System.Drawing.Point(312, 1);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(14, 14);
			this.closeButton.TabIndex = 1;
			this.closeButton.TabStop = false;
			this.closeButton.Text = "x";
			this.toolTip1.SetToolTip(this.closeButton, "Close");
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
			// 
			// captionLabel
			// 
			this.captionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.captionLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.captionLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.captionLabel.Location = new System.Drawing.Point(5, 0);
			this.captionLabel.Name = "captionLabel";
			this.captionLabel.Size = new System.Drawing.Size(323, 16);
			this.captionLabel.TabIndex = 2;
			this.captionLabel.Text = "Output";
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.copyAllMenuItem,
																						 this.copySelectionMenuItem,
																						 this.menuItem1,
																						 this.clearMenuItem});
			// 
			// clearMenuItem
			// 
			this.clearMenuItem.Index = 3;
			this.clearMenuItem.Text = "Clear";
			this.clearMenuItem.Click += new System.EventHandler(this.clearMenuItem_Click);
			// 
			// copyAllMenuItem
			// 
			this.copyAllMenuItem.Index = 0;
			this.copyAllMenuItem.Text = "Copy All";
			this.copyAllMenuItem.Click += new System.EventHandler(this.copyAllMenuItem_Click);
			// 
			// copySelectionMenuItem
			// 
			this.copySelectionMenuItem.Enabled = false;
			this.copySelectionMenuItem.Index = 1;
			this.copySelectionMenuItem.Text = "Copy Selection";
			this.copySelectionMenuItem.Click += new System.EventHandler(this.copySelectionMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// TraceWindowControl
			// 
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.captionLabel);
			this.Name = "TraceWindowControl";
			this.Size = new System.Drawing.Size(328, 240);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Raises the VisibleChanged event
		/// </summary>
		/// <param name="e">event arguments</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			if ( AutoConnect )
			{
				if ( this.Visible )
					Connect();
				else
					Disconnect();
			}
			base.OnVisibleChanged (e);
		}

		private TextWriterTraceListener listener = null;

		/// <summary>
		/// Connects the control to trace events
		/// </summary>
		public void Connect()
		{
			listener = new TextWriterTraceListener( new TextBoxWriter( this.richTextBox1 ) );

			Trace.Listeners.Add( listener );
		}

		/// <summary>
		/// Disconnects the control from trace events
		/// </summary>
		public void Disconnect()
		{
			if ( listener != null )
			{
				Trace.Listeners.Remove( listener );
				listener.Flush();
				listener.Close();
				listener = null;
			}
		}

		/// <summary>
		/// Raised when the close button is clicked
		/// </summary>
		public event EventHandler CloseClick;

		/// <summary>
		/// Raises the <see cref="CloseClick"/> event
		/// </summary>
		protected virtual void OnCloseClick()
		{
			if ( CloseClick != null )
				CloseClick( this, EventArgs.Empty );
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			this.Visible = false;
			OnCloseClick();
		}

		private void closeButton_MouseLeave(object sender, System.EventArgs e)
		{
			if ( this.Focused )
				this.Parent.Focus();
		}

		private void copySelectionMenuItem_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject( this.richTextBox1.SelectedText, true );
		}

		private void copyAllMenuItem_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject( this.richTextBox1.Text, true );
		}

		private void clearMenuItem_Click(object sender, System.EventArgs e)
		{
			this.richTextBox1.Clear();
		}

		private void richTextBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
			{
				this.clearMenuItem.Enabled = this.richTextBox1.Text.Length > 0;
				this.copyAllMenuItem.Enabled = this.richTextBox1.Text.Length > 0;
				this.copySelectionMenuItem.Enabled = this.richTextBox1.SelectedText.Length > 0;		
			}
		}
	}
}

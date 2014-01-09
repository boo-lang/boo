using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace NDoc.Gui
{
	/// <summary>
	/// Form used to display errors to the user
	/// </summary>
	public class ErrorForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox m_messageTextBox;
		private System.Windows.Forms.Button m_closeButton;
		private System.Windows.Forms.Label m_stackTraceLabel;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.TextBox m_stackTraceTextBox;

		private string ClipboardMsg;

		/// <summary>
		/// Show the ErrorForm
		/// </summary>
		/// <param name="message">A message to display</param>
		/// <param name="ex">The exception to display</param>
		/// <param name="parent">The parent control</param>
		public static void ShowError( string message, Exception ex, Control parent )
		{
			if ( parent != null && parent.IsDisposed == false )
			{
				using ( ErrorForm errorForm = new ErrorForm( message, ex ) )
				{
					errorForm.StartPosition = FormStartPosition.CenterParent;
					errorForm.ShowDialog(parent);
				}
			}
		}

		/// <summary>
		/// Show the ErrorForm
		/// </summary>
		/// <param name="ex">The exception to display</param>
		/// <param name="parent">The parent control</param>
		public static void ShowError( Exception ex, Control parent )
		{
			ShowError( ex.Message, ex, parent );
		}

		private ErrorForm(string message, Exception ex)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Icon = SystemIcons.Error;

			StringBuilder strBld = new StringBuilder();
			if ((message != null) && (message.Length > 0))
				strBld.Append(message);

			App.DumpException( strBld, ex );

			string[] lines = strBld.ToString().Split('\n');
			m_messageTextBox.Lines = lines;

			if (ex != null) 
			{
				strBld.Remove(0, strBld.Length);
				Exception tmpEx = ex;
				while (tmpEx != null)
				{
					strBld.AppendFormat("Exception: {0}\n", tmpEx.GetType().ToString());
					strBld.Append(tmpEx.StackTrace);
					strBld.Append("\n\n");
					tmpEx = tmpEx.InnerException;
				}
				lines = strBld.ToString().Split('\n');
				m_stackTraceTextBox.Lines = lines;
			}

			ClipboardMsg = m_messageTextBox.Text + m_stackTraceTextBox.Text;
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
			this.m_messageTextBox = new System.Windows.Forms.TextBox();
			this.m_closeButton = new System.Windows.Forms.Button();
			this.m_stackTraceLabel = new System.Windows.Forms.Label();
			this.m_stackTraceTextBox = new System.Windows.Forms.TextBox();
			this.btnCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_messageTextBox
			// 
			this.m_messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_messageTextBox.Location = new System.Drawing.Point(8, 8);
			this.m_messageTextBox.Multiline = true;
			this.m_messageTextBox.Name = "m_messageTextBox";
			this.m_messageTextBox.ReadOnly = true;
			this.m_messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_messageTextBox.Size = new System.Drawing.Size(616, 160);
			this.m_messageTextBox.TabIndex = 6;
			this.m_messageTextBox.Text = "";
			// 
			// m_closeButton
			// 
			this.m_closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.m_closeButton.Location = new System.Drawing.Point(536, 408);
			this.m_closeButton.Name = "m_closeButton";
			this.m_closeButton.Size = new System.Drawing.Size(88, 24);
			this.m_closeButton.TabIndex = 4;
			this.m_closeButton.Text = "&Close";
			// 
			// m_stackTraceLabel
			// 
			this.m_stackTraceLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.m_stackTraceLabel.Location = new System.Drawing.Point(8, 176);
			this.m_stackTraceLabel.Name = "m_stackTraceLabel";
			this.m_stackTraceLabel.Size = new System.Drawing.Size(88, 16);
			this.m_stackTraceLabel.TabIndex = 8;
			this.m_stackTraceLabel.Text = "Stack Trace:";
			// 
			// m_stackTraceTextBox
			// 
			this.m_stackTraceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_stackTraceTextBox.Location = new System.Drawing.Point(8, 192);
			this.m_stackTraceTextBox.Multiline = true;
			this.m_stackTraceTextBox.Name = "m_stackTraceTextBox";
			this.m_stackTraceTextBox.ReadOnly = true;
			this.m_stackTraceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.m_stackTraceTextBox.Size = new System.Drawing.Size(616, 208);
			this.m_stackTraceTextBox.TabIndex = 7;
			this.m_stackTraceTextBox.Text = "";
			this.m_stackTraceTextBox.WordWrap = false;
			// 
			// btnCopy
			// 
			this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCopy.Location = new System.Drawing.Point(8, 408);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(104, 24);
			this.btnCopy.TabIndex = 9;
			this.btnCopy.Text = "Copy to clipboard";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// ErrorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(632, 450);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.m_messageTextBox);
			this.Controls.Add(this.m_stackTraceTextBox);
			this.Controls.Add(this.m_closeButton);
			this.Controls.Add(this.m_stackTraceLabel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 300);
			this.Name = "ErrorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NDoc Error";
			this.Load += new System.EventHandler(this.ErrorForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void ErrorForm_Load(object sender, System.EventArgs e)
		{
			m_closeButton.Focus();
		}

		private void btnCopy_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(ClipboardMsg,true);
		}

	}
}

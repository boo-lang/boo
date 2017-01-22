using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NDoc.Gui
{
	/// <summary>
	/// Form used to diaply warning messages to the user
	/// </summary>
	public class WarningForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.TextBox txtMessage;

		private string ClipboardMsg;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Shows the warning form
		/// </summary>
		/// <param name="title">The Form's title</param>
		/// <param name="message">The warning</param>
		/// <param name="parent">The Form's parent</param>
		public static void ShowWarning( string title, string message, Control parent )
		{
			if ( parent != null && parent.IsDisposed == false )
			{
				using ( WarningForm warningForm = new WarningForm( title, message ) )
				{
					warningForm.StartPosition = FormStartPosition.CenterParent;
					warningForm.ShowDialog( parent );
				}
			}
		}

		private WarningForm(string title, string message)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Icon = SystemIcons.Warning;

			// add some extra spacing
			lblTitle.Text=title.Replace(" ","  ");
			//massage the text so that WinForms understands the NewLine..
			txtMessage.Text=message.Replace("\n",System.Environment.NewLine);
			txtMessage.Text=txtMessage.Text.Replace("\r\n\n","\r\n");

			ClipboardMsg= title + "\n\n" + message;
			ClipboardMsg=ClipboardMsg.Replace("\n",System.Environment.NewLine);
			ClipboardMsg=ClipboardMsg.Replace("\r\n\n","\r\n");
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WarningForm));
			this.lblTitle = new System.Windows.Forms.Label();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(8, 8);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(464, 24);
			this.lblTitle.TabIndex = 0;
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Location = new System.Drawing.Point(8, 32);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtMessage.Size = new System.Drawing.Size(464, 240);
			this.txtMessage.TabIndex = 2;
			this.txtMessage.TabStop = false;
			this.txtMessage.Text = "";
			this.txtMessage.WordWrap = false;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(384, 280);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(88, 24);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "Close";
			// 
			// btnCopy
			// 
			this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCopy.Location = new System.Drawing.Point(8, 280);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(112, 24);
			this.btnCopy.TabIndex = 4;
			this.btnCopy.Text = "&Copy to clipboard";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// WarningForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(480, 318);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.lblTitle);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(224, 224);
			this.Name = "WarningForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "NDoc Warning";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnCopy_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(ClipboardMsg,true);
		}
	}
}

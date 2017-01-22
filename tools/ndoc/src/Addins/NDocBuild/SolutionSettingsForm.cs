using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using EnvDTE;
using NDoc.Core;
using NDoc.Documenter.Msdn;
using NDoc.Documenter.Xml;

namespace NDoc.Addins.NDocBuild
{
	/// <summary>
	/// Summary description for SolutionSettingsForm.
	/// </summary>
	public class SolutionSettingsForm : System.Windows.Forms.Form
	{
		private NDoc.Core.Project ndocProject;
		private MsdnDocumenter msdnDocumenter;

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Button buttonNamespaceSummaries;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor for the solution settings form.  Sets the property grid
		/// to the MSDN Documenter's configuration properties.
		/// </summary>
		/// <param name="project">an NDoc project for the currently loaded solution.</param>
		public SolutionSettingsForm(NDoc.Core.Project project)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ndocProject = project;
			msdnDocumenter = new MsdnDocumenter();

			this.SetStyle(ControlStyles.DoubleBuffer, true);
			propertyGrid.SelectedObject = ndocProject.GetDocumenter(msdnDocumenter.Name).Config;
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.buttonNamespaceSummaries = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(408, 16);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 0;
			this.okButton.Text = "Ok";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(408, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(16, 16);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(376, 335);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.Text = "PropertyGrid";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// buttonNamespaceSummaries
			// 
			this.buttonNamespaceSummaries.Location = new System.Drawing.Point(408, 120);
			this.buttonNamespaceSummaries.Name = "buttonNamespaceSummaries";
			this.buttonNamespaceSummaries.Size = new System.Drawing.Size(75, 40);
			this.buttonNamespaceSummaries.TabIndex = 2;
			this.buttonNamespaceSummaries.Text = "Namespace Summaries";
			this.buttonNamespaceSummaries.Click += new System.EventHandler(this.buttonNamespaceSummaries_Click);
			// 
			// SolutionSettingsForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(498, 368);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonNamespaceSummaries,
																		  this.cancelButton,
																		  this.okButton,
																		  this.propertyGrid});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SolutionSettingsForm";
			this.Text = "NDoc Solution Settings";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonNamespaceSummaries_Click(object sender, System.EventArgs e)
		{
			try
			{
				NamespaceSummariesForm formNamespaceSummaries = new NamespaceSummariesForm(ndocProject);
				formNamespaceSummaries.ShowDialog();
			}
			catch(Exception)
			{
				MessageBox.Show("An error occurred in the Namespace Summaries form.");
			}
		}
	}
}

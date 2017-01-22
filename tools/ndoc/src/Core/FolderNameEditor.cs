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
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

using NDoc.ExtendedUI;


namespace NDoc.Core.PropertyGridUI
{
	/// <summary>
	/// 
	/// </summary>
	public class FoldernameEditor : System.Drawing.Design.UITypeEditor
	{
		private string _InitialFolder = String.Empty;

		/// <summary>
		/// Gets the edit style.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return UITypeEditorEditStyle.None;
		}

		/// <summary>
		/// Edits the value.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="provider">Provider.</param>
		/// <param name="value">Value.</param>
		/// <returns></returns>
		[RefreshProperties(RefreshProperties.All)] 
		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) 
		{
			if (context == null || provider == null || context.Instance == null) 
			{
				return base.EditValue(provider, value);
			}

			ShellBrowseForFolderDialog folderDialog = new ShellBrowseForFolderDialog();
			try
			{
				folderDialog.hwndOwner = System.Windows.Forms.Form.ActiveForm.Handle;
			}
			catch{}

			_InitialFolder=(string)value;

			FolderDialogTitleAttribute titleAtt = (FolderDialogTitleAttribute)context.PropertyDescriptor.Attributes[typeof(FolderDialogTitleAttribute)];
			if (titleAtt != null) 
			{
				folderDialog.Title = titleAtt.Title;
			}

			folderDialog.OnInitialized += new ShellBrowseForFolderDialog.InitializedHandler(this.InitializedEvent);
			if (folderDialog.ShowDialog() == DialogResult.OK) 
			{
				value = folderDialog.FullName;
			}
			return value;
		}

		private void InitializedEvent(ShellBrowseForFolderDialog sender, ShellBrowseForFolderDialog.InitializedEventArgs args)
		{
			sender.SetSelection(args.hwnd, _InitialFolder);
		}

		#region " FolderDialogTitle Attribute"
		/// <summary>
		/// 
		/// </summary>
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
			public class FolderDialogTitleAttribute : Attribute
		{
			private string _title;
			/// <summary>
			/// Gets the title.
			/// </summary>
			/// <value></value>
			public string Title
			{
				get { return _title; }
			}

			/// <summary>
			/// Define a title for the UIFoldernameEditor.
			/// </summary>
			/// <param name="title">Folder dialog title</param>
			/// <remarks></remarks>
			public FolderDialogTitleAttribute(string title) : base() 
			{
				this._title = title;
			}
		}
		
		/// <summary>
		/// Indicates that only existing folders can be specified
		/// </summary>
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
			public class ExistingFolderOnlyAttribute : Attribute {}
		#endregion
	}
}

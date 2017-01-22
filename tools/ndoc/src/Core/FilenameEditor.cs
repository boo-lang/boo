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
using System.IO;


namespace NDoc.Core.PropertyGridUI
{
	/// <summary>
	/// 
	/// </summary>
	public class FilenameEditor : System.Drawing.Design.UITypeEditor
	{

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

			string fileName = String.Empty;
			string initialDirectory= String.Empty;

			if (((string)value).Length>0)
			{
				fileName = Path.GetFileName((string)value);
				initialDirectory = Path.GetDirectoryName((string)value);
				if(initialDirectory==null)initialDirectory=(string)value;
			}

			FileDialog fileDlg;
			if (context.PropertyDescriptor.Attributes[typeof(SaveFileAttribute)] == null) 
			{
				fileDlg = new OpenFileDialog();
			} 
			else 
			{
				fileDlg = new SaveFileDialog();
			}
			fileDlg.RestoreDirectory=true;
			fileDlg.FileName = fileName;
			fileDlg.InitialDirectory = initialDirectory;

			FileDialogFilterAttribute filterAtt = (FileDialogFilterAttribute)context.PropertyDescriptor.Attributes[typeof(FileDialogFilterAttribute)];
			if (filterAtt != null) 
			{
				fileDlg.Title = filterAtt.Title;
				fileDlg.Filter = filterAtt.Filter;
			}
			else
			{
				fileDlg.Title = "Select " + context.PropertyDescriptor.DisplayName;
			}
			if (fileDlg.ShowDialog() == DialogResult.OK) 
			{
				value = fileDlg.FileName;
			}
			fileDlg.Dispose();
			return value;
		}

		#region " Filter attribute "
		/// <summary>
		/// 
		/// </summary>
		[AttributeUsage(AttributeTargets.Property)]
			public class FileDialogFilterAttribute : Attribute
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

			private string _filter;

			/// <summary>
			/// The filter to use in the file dialog in UIFilenameEditor.
			/// </summary>
			/// <value></value>
			/// <remarks>
			/// The following is an example of a filter string: 
			/// "Text files (*.txt)|*.txt|All files (*.*)|*.*"
			/// </remarks>
			public string Filter 
			{
				get 
				{
					return this._filter;
				}
			}

			/// <summary>
			/// Define a filter for the UIFilenameEditor.
			/// </summary>
			/// <param name="title">File dialog title</param>
			/// <param name="filter">
			/// The filter to use in the file dialog in UIFilenameEditor. 
			/// The following is an example of a filter string: 
			/// "Text files (*.txt)|*.txt|All files (*.*)|*.*"</param>
			/// <remarks></remarks>
			public FileDialogFilterAttribute(string title, string filter) : base() 
			{
				this._title = title;
				this._filter = filter;
			}
		}
		#endregion

		#region " Save file attribute - indicates that SaveFileDialog must be shown "
		/// <summary>
		/// Indicates that SaveFileDialog must be shown
		/// </summary>
		[AttributeUsage(AttributeTargets.Property)]
			public class SaveFileAttribute : Attribute {}
		#endregion

	}
}

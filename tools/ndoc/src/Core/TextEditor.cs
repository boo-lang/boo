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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace NDoc.Core.PropertyGridUI
{
	/// <summary>
	/// Provides editing facilities for large blocks of text in the <see cref="PropertyGrid"/>.
	/// </summary>
	public class TextEditor : UITypeEditor
	{
		/// <summary>
		/// Creates a new instance of the <see cref="TextEditor"/> class.
		/// </summary>
		public TextEditor()
		{
		}

		/// <summary>
		/// Edits the specified object's value using the editor style indicated by <see cref="GetEditStyle(ITypeDescriptorContext)"/>.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
		/// <param name="provider">An <see cref="IServiceProvider"/> that this editor can use to obtain services.</param>
		/// <param name="value">The object to edit.</param>
		/// <returns>The new value of the object.</returns>
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null)
			{
				TextEditorForm form = new TextEditorForm();
				form.Value = (string)value;
				DialogResult result = form.ShowDialog();

				if (result == DialogResult.OK)
					value = form.Value;
			}

			return value;
		}

		/// <summary>
		/// Gets the editor style used by the <see cref="EditValue(ITypeDescriptorContext, IServiceProvider, object)"/> method.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
		/// <returns>
		/// A <see cref="UITypeEditorEditStyle"/> value that indicates
		/// the style of editor used by <see cref="EditValue(ITypeDescriptorContext, IServiceProvider, object)"/>.
		/// </returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;

			return base.GetEditStyle(context);
		}
	}
}

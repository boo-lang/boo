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
	/// Class which implements a custom UITypeEditor for attributes.
	/// </summary>
	public class AttributesEditor : System.Drawing.Design.UITypeEditor 
	{
		/// <summary>
		/// Handler called when editing a value.
		/// </summary>
		/// <param name="context">Context</param>
		/// <param name="provider">Provider</param>
		/// <param name="value">Current Value</param>
		/// <returns>New value</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
			if (context != null
				&& context.Instance != null
				&& provider != null) 
			{
				AttributesForm dlg = new AttributesForm(value);
				if(dlg.ShowDialog() == DialogResult.OK)
				{
					return dlg.Value;
				}
			}

			return value;
		}

		/// <summary>
		/// Returns the edit style for the type.
		/// </summary>
		/// <param name="context">Context</param>
		/// <returns>Edit Style</returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}

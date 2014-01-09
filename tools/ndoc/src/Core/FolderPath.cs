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
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.IO;

using NDoc.Core.PropertyGridUI;

namespace NDoc.Core
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[DefaultProperty("Path")]
	[TypeConverter(typeof(FolderPath.TypeConverter))]
	[Editor(typeof(FolderPath.UIEditor), typeof(UITypeEditor))]
	public class FolderPath : PathItemBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FolderPath"/> class.
		/// </summary>
		public FolderPath() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="FolderPath"/> class from a given path string.
		/// </summary>
		/// <param name="path">Path.</param>
		public FolderPath(string path) : base(path)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FolderPath"/> class from an existing <see cref="FilePath"/> instance.
		/// </summary>
		/// <param name="path">An existing <see cref="FolderPath"/>.</param>
		public FolderPath(FolderPath path)
		{
			if (path.Path.Length > 0)
			{
				base.Path = path.Path;
				base.FixedPath = path.FixedPath;
			}
		}

		/// <inheritDoc/>
		[ReadOnly(true)]
		public override string Path
		{
			get { return base.Path; }
			set 
			{ 
				if (value.Length > 0)
				{
					base.Path = value;
				}
				else
				{
					base.SetPathInternal(String.Empty);
				}
			}
		}

		/// <inheritDoc/>
		public override string ToString()
		{
			string path = base.ToString();
			if (path.Length > 0)
			{
				if (!path.EndsWith(new String(System.IO.Path.DirectorySeparatorChar, 1)) && 
					!path.EndsWith(new String(System.IO.Path.AltDirectorySeparatorChar, 1)) && 
					!path.EndsWith(new String(System.IO.Path.VolumeSeparatorChar, 1))
					)
				{
					path = path + new String(System.IO.Path.DirectorySeparatorChar, 1);
				}
			}
			return path;
		}

		// This is a special type converter which will be associated with the FolderPath class.
		// It converts a string representation to a FolderPath object for use in a property grid.
		/// <summary>
		/// 
		/// </summary>
		new internal class TypeConverter : PropertySorter
		{
			/// <inheritDoc/>
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof(string)) 
				{
					return true;
				}
				return base.CanConvertFrom(context, sourceType);
			}
		
			/// <inheritDoc/>
			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				if (value is string) 
				{
					return new FolderPath((string)value);
				}
				return base.ConvertFrom(context, culture, value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal class UIEditor : FoldernameEditor
		{
			/// <summary>
			/// Creates a new <see cref="UIEditor">FolderPath.UIEditor</see> instance.
			/// </summary>
			public UIEditor() : base() {}
	
			/// <inheritDoc/>
			public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (value is FolderPath)
				{
					object result = base.EditValue(context, provider, ((FolderPath)value).Path);
					if ((string)result == ((FolderPath)value).Path)
					{
						return value;
					}
					else
					{
						if (((string)result).Length > 0)
						{
							FolderPath newValue = new FolderPath((FolderPath)value);
							newValue.Path = (string)result;
							return newValue;
						}
						else
						{
							return new FolderPath();
						}
					}
				}
				else
				{
					return base.EditValue(context, provider, value);
				}
			}
		}
	}
}

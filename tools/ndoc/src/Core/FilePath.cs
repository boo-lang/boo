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
	[TypeConverter(typeof(FilePath.TypeConverter))]
	[Editor(typeof(FilePath.UIEditor), typeof(UITypeEditor))]
	public class FilePath : PathItemBase
	{
		/// <summary>Initializes a new instance of the <see cref="FilePath"/> class.</summary>
		/// <overloads>Initializes a new instance of the <see cref="FilePath"/> class.</overloads>
		public FilePath() : base() {}

		/// <summary>Initializes a new instance of the <see cref="FilePath"/> class from a given path string.</summary>
		/// <param name="path">Path.</param>
		public FilePath(string path) : base(path)
		{
			Path = path;
		}

		/// <summary>Initializes a new instance of the <see cref="FilePath"/> class from an existing <see cref="FilePath"/> instance.</summary>
		/// <param name="path">An existing <see cref="FilePath"/>.</param>
		public FilePath(FilePath path)
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

		// This is a special type converter which will be associated with the FilePath class.
		// It converts an FilePath object to a string representation for use in a property grid.
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
					return new FilePath((string)value);
				}
				return base.ConvertFrom(context, culture, value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal class UIEditor : FilenameEditor
		{
			/// <summary>
			/// Creates a new <see cref="UIEditor">FilePath.UIEditor</see> instance.
			/// </summary>
			public UIEditor() : base() {}
	
			/// <inheritDoc/>
			public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (value is FilePath)
				{
					object result = base.EditValue(context, provider, ((FilePath)value).Path);
					if ((string)result == ((FilePath)value).Path)
					{
						return value;
					}
					else
					{
						if (((string)result).Length > 0)
						{
							FilePath newValue = new FilePath((FilePath)value);
							newValue.Path = (string)result;
							return newValue;
						}
						else
						{
							return new FilePath();
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

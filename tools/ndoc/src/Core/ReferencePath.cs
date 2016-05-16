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
	/// A path to search for referenced assemblies.
	/// </summary>
	/// <remarks>
	/// if <see cref="IncludeSubDirectories"/> is set to <see langword="true"/>, subdirectories of 
	/// <see cref="Path"/> will also be searched.
	/// </remarks>
	[Serializable]
	[DefaultProperty("Path")]
	[TypeConverter(typeof(ReferencePath.TypeConverter))]
	[Editor(typeof(ReferencePath.UIEditor), typeof(UITypeEditor))]
	[NDoc.Core.PropertyGridUI.FoldernameEditor.FolderDialogTitle("Select Reference Path")]
	public class ReferencePath : PathItemBase
	{
		#region Private Fields
		private bool _IncludeSubDirectories = false;
		#endregion

		#region Constructors
		/// <overloads>
		/// Initializes a new instance of the <see cref="ReferencePath"/> class.
		/// </overloads>
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencePath"/> class.
		/// </summary>
		public ReferencePath() {}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencePath"/> class from a given path string.
		/// </summary>
		/// <param name="path">A relative or absolute path.</param>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> is a <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="path"/> is an empty string.</exception>
		/// <remarks>
		/// If <paramref name="path"/> end with "**" then <see cref="IncludeSubDirectories"/> 
		/// will be set to <see langword="true"/>.
		/// </remarks>
		public ReferencePath(string path)
		{
			if (path == null)
				throw new ArgumentNullException("Path");

			if (path.Length == 0)
				throw new ArgumentOutOfRangeException("path", "path must not be empty.");

			if (path.EndsWith("**"))
			{
				_IncludeSubDirectories = true;
				path = path.Substring(0, path.Length - 2);
			}
			Path = path;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencePath"/> class from an existing <see cref="ReferencePath"/> instance.
		/// </summary>
		/// <param name="refPath">An existing <see cref="ReferencePath"/> instance.</param>
		/// <exception cref="ArgumentNullException"><paramref name="refPath"/> is a <see langword="null"/>.</exception>
		public ReferencePath(ReferencePath refPath)
		{
			if (refPath == null)
				throw new ArgumentNullException("refPath");

			base.Path = refPath.Path;
			base.FixedPath = refPath.FixedPath;
			this._IncludeSubDirectories = refPath._IncludeSubDirectories;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the fully qualified path.
		/// </summary>
		/// <value>The fully qualified path</value>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> is a <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="path"/> is an empty string.</exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="path"/> is not rooted, <see cref="PathItemBase.FixedPath"/> is set to <see langword="false"/>, otherwise
		/// it left at its current setting.
		/// </para>
		/// <para>
		/// If this property is set to a string that ends with "**" then <see cref="IncludeSubDirectories"/> 
		/// will be set to <see langword="true"/>.
		/// </para>
		/// </remarks>
		[ReadOnly(true)]
		[Description("A path to search for referenced assemblies.")]
		[MergableProperty(false)]
		[PropertyOrder(10)]
		[RefreshProperties(RefreshProperties.All)] 
		public override string Path
		{
			get { return base.Path; }
			set 
			{ 
				if (value == null)
					throw new ArgumentNullException("Path");

				if (value == null)
					throw new ArgumentOutOfRangeException("Path", "path must not be empty.");

				if (value.EndsWith("**"))
				{
					_IncludeSubDirectories = true;
					value = value.Substring(0, value.Length - 2);
				}
				base.Path = value;
			}
		}

		/// <summary>
		/// Gets or sets an indication whether to search subdirectories of the given path.
		/// </summary>
		/// <value>
		/// if <see langword="true"/>, the assembly loader will search subdirectories; otherwise, it will only search given path.
		/// </value>
		[Description("If true, the assembly loader will search subdirectories; otherwise, it will only search given path.")]
		[PropertyOrder(100)]
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)] 
		public bool IncludeSubDirectories
		{
			get { return _IncludeSubDirectories; }
			set { _IncludeSubDirectories = value; }
		}

		#endregion

		#region Equality	
		/// <inheritDoc/>
		public override bool Equals(object obj)
		{
			if (!base.Equals(obj)) return false;

			ReferencePath rp = (ReferencePath) obj;
			if (!this._IncludeSubDirectories.Equals(rp._IncludeSubDirectories)) return false;
			return true;
		}
		/// <summary>Equality operator.</summary>
		public static bool operator == (ReferencePath x, ReferencePath y) 
		{ 
			if ((object)x == null) return false;
			return x.Equals(y);
		}
		/// <summary>Inequality operator.</summary>
		public static bool operator != (ReferencePath x, ReferencePath y) 
		{ 
			return!(x == y);
		}
	
		/// <inheritDoc/>
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		#endregion


		/// <inheritDoc/>
		public override string ToString()
		{
			string displayPath = AppendSubDirInd(base.ToString(), _IncludeSubDirectories);

			return displayPath;
		}

		#region Helpers

		private string AppendSubDirInd(string path, bool includeSubDirectories)
		{
			string displayPath = path;

			if (includeSubDirectories)
			{
				if (path.EndsWith(new String(System.IO.Path.DirectorySeparatorChar, 1)) || 
					path.EndsWith(new String(System.IO.Path.AltDirectorySeparatorChar, 1)) || 
					path.EndsWith(new String(System.IO.Path.VolumeSeparatorChar, 1))
					)
					displayPath = displayPath + "**";
				else
					displayPath = displayPath + System.IO.Path.DirectorySeparatorChar + "**";
			}
			return displayPath;
		}

		#endregion

		/// <summary>
		/// <see cref="TypeConverter"/> to convert a string to an instance of <see cref="ReferencePath"/>.
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
				Debug.WriteLine("RefPath:TypeConv.ConvertFrom  value=>" + value.ToString());
				if (value is string) 
				{
					ReferencePath rp = new ReferencePath((string)value);
					Debug.WriteLine("  rp=>" + rp.ToString());
					return rp;
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
			/// Creates a new <see cref="UIEditor">ReferencePath.UIEditor</see> instance.
			/// </summary>
			public UIEditor() : base() {}
	
			/// <inheritDoc/>
			public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				if (value is ReferencePath)
				{
					object result = base.EditValue(context, provider, ((ReferencePath)value).Path);
					if ((string)result == ((ReferencePath)value).Path)
					{
						return value;
					}
					else
					{
						if (((string)result).Length > 0)
						{
							ReferencePath newValue = new ReferencePath((ReferencePath)value);
							newValue.Path = (string)result;
							return newValue;
						}
						else
						{
							return new ReferencePath();
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


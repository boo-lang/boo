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
	[TypeConverter(typeof(PathItemBase.TypeConverter))]
	public class PathItemBase 
	{
		#region Static Members
		private static string _basePath;
		/// <summary>
		/// The base path for converting <see cref="PathItemBase"/> path to relative form.
		/// </summary>
		/// <remarks>
		/// If the path has not been explicitly set, it defaults to the working directory.
		/// </remarks>
		public static string BasePath
		{
			get 
			{
				if ((_basePath != null) && (_basePath.Length > 0))
				{
					return _basePath;
				}
				else
				{
					return Directory.GetCurrentDirectory();
				}
			}
			set 
			{
				_basePath = value;
			}
		}

		/// <summary>
		/// Explicit conversion of <see cref="PathItemBase"/> to <see cref="String"/>.
		/// </summary>
		/// <param name="path">The <see cref="PathItemBase"/> to convert.</param>
		/// <returns>A string containg the fully-qualified path contained in the passed <see cref="PathItemBase"/>.</returns>
		public static implicit operator String(PathItemBase path)
		{
			return path._Path;
		}

		#endregion

		#region Private Fields
		private string _Path = "";
		private bool _FixedPath = false;
		#endregion

		#region Constructors
		/// <overloads>
		/// Initializes a new instance of the <see cref="PathItemBase"/> class.
		/// </overloads>
		/// <summary>
		/// Initializes a new instance of the <see cref="PathItemBase"/> class.
		/// </summary>
		public PathItemBase() {}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PathItemBase"/> class from a given path string.
		/// </summary>
		/// <param name="path">A relative or absolute path.</param>
		/// <exception cref="ArgumentNullException"><paramref name="path"/> is a <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="path"/> is an empty string.</exception>
		/// <remarks>
		/// If a <paramref name="path"/> is rooted, <see cref="FixedPath"/> is set to <see langword="true"/>, otherwise
		/// is is set to <see langword="false"/>
		/// </remarks>
		public PathItemBase(string path)
		{
			if (path == null)
				throw new ArgumentNullException("Path");

			if (path.Length > 0)
			{
				if (!System.IO.Path.IsPathRooted(path))
				{
					path = PathUtilities.RelativeToAbsolutePath(BasePath, path);
					this.FixedPath = false;
				}
				else
				{
					this.FixedPath = true;
				}
				_Path = path;
			}
			else
			{
				_Path = "";
				_FixedPath = false;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PathItemBase"/> class from an existing <see cref="PathItemBase"/> instance.
		/// </summary>
		/// <param name="pathItemBase">An existing <see cref="PathItemBase"/> instance.</param>
		/// <exception cref="ArgumentNullException"><paramref name="pathItemBase"/> is a <see langword="null"/>.</exception>
		public PathItemBase(PathItemBase pathItemBase)
		{
			if (pathItemBase == null)
				throw new ArgumentNullException("pathItemBase");

			this._Path = pathItemBase._Path;
			this._FixedPath = pathItemBase._FixedPath;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the fully qualified path.
		/// </summary>
		/// <value>The fully qualified path</value>
		/// <exception cref="ArgumentNullException">set <paramref name="value"/> is a <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">set <paramref name="value"/> is an empty string.</exception>
		/// <remarks>
		/// If the set path is not rooted, <see cref="FixedPath"/> is set to <see langword="false"/>, otherwise
		/// it left at its current setting.
		/// </remarks>
		[MergableProperty(false)]
		[PropertyOrder(10)]
		public virtual string Path
		{
			get { return _Path; }
			set 
			{ 
				if (value == null)
					throw new ArgumentNullException("Path");

				if (value.Length == 0)
					throw new ArgumentOutOfRangeException("Path", "path must not be empty.");

				if (!System.IO.Path.IsPathRooted(value))
				{
					value = PathUtilities.RelativeToAbsolutePath(BasePath, value);
					this.FixedPath = false;
				}
				_Path = value;
			}
		}

		/// <summary>
		/// Gets or sets an indication whether the path should be saved as fixed or relative to the project file.
		/// </summary>
		/// <value>
		/// if <see langword="true"/>, NDoc will save this as a Fixed path; 
		/// otherwise, it will be saved as a path relative to the NDoc project file.
		/// </value>
		[Description("If true, NDoc will save this as a fixed path; otherwise, it will be saved as a path relative to the NDoc project file.")]
		[DefaultValue(false)]
		[PropertyOrder(20)]
		[RefreshProperties(RefreshProperties.Repaint)] 
		public bool FixedPath
		{
			get { return _FixedPath; }
			set { _FixedPath = value; }
		}

		#endregion


		internal void SetPathInternal(string path)
		{
			_Path = path;
		}

		/// <inheritDoc/>
		public override string ToString()
		{
			string displayPath = PersistablePath(BasePath);
			return displayPath;
		}

		#region Equality	
		/// <inheritDoc/>
		public override bool Equals(object obj)
		{
			PathItemBase testObj = obj as PathItemBase;

			if (obj == null) return false;
			if (this.GetType() != obj.GetType()) return false;

			if (!Object.Equals(this._Path, testObj._Path)) return false;
			if (!_FixedPath.Equals(testObj._FixedPath)) return false;
			return true;
		}
		/// <summary>Equality operator.</summary>
		public static bool operator == (PathItemBase x, PathItemBase y) 
		{ 
			if ((object)x == null) return false;
			return x.Equals(y);
		}
		/// <summary>Inequality operator.</summary>
		public static bool operator != (PathItemBase x, PathItemBase y) 
		{ 
			return!(x == y);
		}
	
		/// <inheritDoc/>
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		#endregion

		#region Helpers
		internal string PersistablePath(string basePath)
		{
			string displayPath = _Path;
			if (FixedPath)
			{
				displayPath = FixPath(displayPath);
			}
			else
			{
				displayPath = RelativePath(displayPath);
			}
			return displayPath;
		}

		private string FixPath(string path)
		{
			if (System.IO.Path.IsPathRooted(path))
			{
				return path;
			}
			else
			{
				return PathUtilities.RelativeToAbsolutePath(BasePath, path);
			}
		}

		private string RelativePath(string path)
		{
			if (System.IO.Path.IsPathRooted(path))
			{
				return PathUtilities.AbsoluteToRelativePath(BasePath, path);
			}
			else
			{
				return path;
			}
		}

		#endregion

		// This is a special type converter which will be associated with the PathItemBase class.
		// It converts an PathItemBase object to a string representation for use in a property grid.
		/// <summary>
		/// 
		/// </summary>
		internal class TypeConverter : PropertySorter
		{
			/// <inheritDoc/>
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
			{
				if (destType == typeof(string) && value is PathItemBase)
				{
					return value.ToString();
				}
				return base.ConvertTo(context, culture, value, destType);
			}
		
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
					return new PathItemBase((string)value);
				}
				return base.ConvertFrom(context, culture, value);
			}
		}
	}
}

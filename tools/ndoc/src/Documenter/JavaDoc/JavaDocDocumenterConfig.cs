// JavaDocDocumenterConfig.cs - the JavaDoc documenter config class
// Copyright (C) 2001  Jason Diamond
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
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.JavaDoc
{
	/// <summary>The JavaDoc documenter config class.</summary>
	[DefaultProperty("OutputDirectory")]
	public class JavaDocDocumenterConfig : BaseReflectionDocumenterConfig
	{
		/// <summary>Initializes a new instance of the JavaDocDocumenterConfig class.</summary>
		/// <remarks>
		/// <para>The JavaDoc documenter is used to make a set of HTML documentation
		/// similar in format and layout to the documentation created by Java's JavaDoc
		/// technology.</para>
		/// <para><i>Due to lack of interest this documenter is not under active development.</i>
		/// If you are interested in updating this documenter please 
		/// <a href="http://sourceforge.net/projects/ndoc/">contact one of NDoc's Admins</a>.
		/// </para>
		/// </remarks>
		public JavaDocDocumenterConfig( JavaDocDocumenterInfo info ) : base( info )
		{
		}

		/// <summary>
		/// Creates an instance of a documenter <see cref="IDocumenterConfig.CreateDocumenter"/>
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		public override IDocumenter CreateDocumenter()
		{
			return new JavaDocDocumenter( this );
		}

		private string _Title;

		/// <summary>Gets or sets the Title property.</summary>
		/// <remarks>The name of the JavaDoc project.</remarks>
		[Category("Documentation Main Settings")]
		[Description("The name of the JavaDoc project.")]
		public string Title
		{
			get
			{
				return _Title;
			}

			set
			{
				_Title = value;
				SetDirty();
			}
		}

		string _outputDirectory = string.Format( ".{0}doc{0}", Path.DirectorySeparatorChar );
		
		/// <summary>Gets or sets the OutputDirectory property.</summary>
		/// <remarks>The folder where the root of the HTML set will be located.
		/// This can be absolute or relative from the .ndoc project file.</remarks>
		[Category("Documentation Main Settings")]
		[Description("The folder where the root of the HTML set will be located.\nThis can be absolute or relative from the .ndoc project file.")]
		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public string OutputDirectory
		{
			get { return _outputDirectory; }

			set
			{
				if ( value.IndexOfAny(new char[]{'#','?', ';'}) != -1) 
				{
					throw new FormatException("Output Directory '" + value + 
						"' is not valid because it contains '#','?' or ';' which" +
						" are reserved characters in HTML URLs."); 
				}

				_outputDirectory = value;

				if (!_outputDirectory.EndsWith( Path.DirectorySeparatorChar.ToString() ))
				{
					_outputDirectory += Path.DirectorySeparatorChar;
				}

				SetDirty();
			}
		}

		//void ResetOutputDirectory() { _outputDirectory = string.Format( ".{0}doc{0}", Path.DirectorySeparatorChar ); }
	}
}

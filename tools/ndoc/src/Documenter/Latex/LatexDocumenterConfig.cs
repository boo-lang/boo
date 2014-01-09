// The LaTeX documenter configuration class.
//
// Copyright (C) 2002 Thong Nguyen (tum_public@veridicus.com)
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

namespace NDoc.Documenter.Latex
{
	/// <summary>
	/// Summary description for LatexDocumenterConfig.
	/// </summary>
	/// <remarks>
	/// <para>The LaTeX documenter can be used to create dvi or postscript documents.</para>
	/// <para>This documenter requires that a LaTeX compiler be installed.
	/// You can download a free one from <a href="http://www.miktex.org">www.MiKTeX.org</a>.</para>
	/// </remarks>
	[DefaultProperty("OutputDirectory")]
	public class LatexDocumenterConfig : BaseReflectionDocumenterConfig
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="info">Info class descrbing the documenter</param>
		public LatexDocumenterConfig( LatexDocumenterInfo info ) : base( info )
		{
			// fix for bug 884121 - OutputDirectory on Linux
			OutputDirectory = string.Format(".{0}doc{0}",Path.DirectorySeparatorChar );
			TexFileBaseName = "Documentation";
			LatexCompiler = "latex";
		}

		/// <summary>
		/// Creates an instance of a documenter <see cref="IDocumenterConfig.CreateDocumenter"/>
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		public override IDocumenter CreateDocumenter()
		{
			return new LatexDocumenter( this );
		}

		/// <summary>Gets or sets the output directory.</summary>
		/// <remarks>The folder documentation will be created. This can be 
		/// absolute or relative from the .ndoc project file.</remarks>
		[Category("LaTeX")]
		[Description("The directory to output the files to.")]
		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public string OutputDirectory
		{
			get
			{
				return m_OutputDirectory;
			}

			set
			{
				m_OutputDirectory = value;
			}
		}
		private string m_OutputDirectory;

		/// <summary>Gets the full name of the LaTeX document.</summary>
		[Category("LaTeX")]
		[Description("Full name of the LaTeX document.")]
		public string TextFileFullName
		{
			get
			{
				return TexFileBaseName + ".tex";
			}
		}
		
		/// <summary>Gets or sets the name of the LaTeX document excluding the file extension.</summary>
		/// <remarks>Name of the LaTeX document, excluding the file extension.</remarks>
		[Category("LaTeX")]
		[Description("Name of the LaTeX document, excluding the file extension.")]
		public string TexFileBaseName
		{
			get
			{
				return m_TexFileBaseName;
			}

			set
			{
				m_TexFileBaseName = value;
			}
		}
		private string m_TexFileBaseName;

		/// <summary>Gets or sets the LaTeX compiler path.</summary>
		/// <remarks>Path to the LaTeX compiler executable (Set to empty if you do not have LaTeX installed).</remarks>
		[Category("LaTeX")]
		[Description("Path to the LaTeX executable (Set to empty if you do not have LaTeX installed).")]
		[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
		public string LatexCompiler
		{
			get
			{
				return m_LatexCompiler;
			}

			set
			{
				m_LatexCompiler = value;
			}
		}
		private string m_LatexCompiler;

		/// <summary>Gets the path of the output file.</summary>
		[Category("LaTeX")]
		[Description("Full path to the output TeX file.")]
		public string TexFileFullPath
		{
			get
			{
				return Path.Combine(OutputDirectory, TextFileFullName);
			}
		}
	}
} 


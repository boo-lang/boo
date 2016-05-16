// XmlDocumenterConfig.cs - XML documenter config class
// Copyright (C) 2001  Kral Ferch, Jason Diamond
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

namespace NDoc.Documenter.Xml
{
	/// <summary>The XmlDocumenter config class.</summary>
	/// <remarks>	
	/// <para>
	/// The XML Documenter is the simplest of the NDoc Documenters. It is primarily 
	/// a development tool.
	/// </para>
	/// <para>
	/// As part of the documentation compile process, NDoc merges the type information 
	///	in the assemblies being documented with the code comment summary XML document that the
	///	<a href="ms-help://MS.NETFrameworkSDKv1.1/cscomp/html/vcerrDocProcessDocumentationComments.htm">
	///	/doc compiler option</a> emits. The XML Documenter allows you to save this merged 
	///	set of data for curiosity&#39;s sake or debugging purposes. 
	///	</para>
	/// <para>
	/// Used in conjunction with the <b>UseNDocXmlFile</b> setting, this is especially 
	///	useful when you are working on your own documenters.
	///	</para>
	///	</remarks>
	[DefaultProperty("OutputFile")]
	public class XmlDocumenterConfig : BaseReflectionDocumenterConfig
	{
		/// <summary>Initializes a new instance of the XmlDocumenterConfig class.</summary>
		public XmlDocumenterConfig( XmlDocumenterInfo info ) : base( info )
		{
			OutputFile = string.Format( ".{0}doc{0}doc.xml", Path.DirectorySeparatorChar );
		}
		
		/// <summary>
		/// Creates an instance of a documenter <see cref="IDocumenterConfig.CreateDocumenter"/>
		/// </summary>
		/// <returns>IDocumenter instance</returns>		
		public override IDocumenter CreateDocumenter()
		{
			return new XmlDocumenter( this );
		}

		private string _OutputFile;

		/// <summary>Gets or sets the OutputFile property.</summary>
		/// <remarks>This is the path and filename of the file where 
		/// the merged /doc output and reflection information will be written. This can be 
		/// absolute or relative from the .ndoc project file.</remarks>
		[Category("Documentation Main Settings")]
		[Description("The path to the XML file to create which will be the combined /doc output and reflection information.")]
		[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
		public string OutputFile
		{
			get { return _OutputFile; }

			set 
			{ 
				_OutputFile = value; 
				SetDirty();
			}
		}

	}
}

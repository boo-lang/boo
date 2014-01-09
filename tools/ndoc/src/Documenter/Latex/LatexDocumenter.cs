// The LaTeX documenter.
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
//


//
// This doucmenter won't be able to generate dvi or pdf files without
// a LaTeX compiler.  You can download a free one from: http://www.miktex.org.
// 
//

using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.Latex
{
	/// <summary>
	/// LaTeX generating documenter class.
	/// </summary>
	public class LatexDocumenter: BaseReflectionDocumenter
	{
		//private string m_ResourceDirectory;

		private static readonly char[] c_LatexEscapeChars =
			{
				'_',
				'$',
				'%',
				'{',
				'}',
				'\\'				
			};

		private static readonly string[] c_TempFileExtensions =
			{				
				".log",
				".aux"
			};

		private static readonly string[] c_KnownLatexOuputExtensions =
			{				
				".pdf",
				".dvi"
			};


		/// <summary>
		/// Constructs a new LaTeX documenter.
		/// </summary>
		/// <remarks>
		/// The documenter name is set to "LaTeX".
		/// </remarks>
		public LatexDocumenter( LatexDocumenterConfig config ) : base( config )
		{
		}

		/// <summary>
		/// Gets the Configuration object for this documenter.
		/// </summary>
		private LatexDocumenterConfig LatexConfig
		{
			get
			{
				return (LatexDocumenterConfig)Config;
			}
		}

		/// <summary>
		/// <see cref="BaseDocumenter.Build"/>
		/// </summary>
		/// <param name="project"></param>
		public override void Build(Project project)
		{		
			Workspace workspace = new LatexWorkspace( WorkingPath );
			workspace.Clean();
			workspace.Prepare();

#if NO_RESOURCES
			// copy all of the xslt source files into the workspace
			DirectoryInfo xsltSource = new DirectoryInfo( Path.GetFullPath(Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\Documenter\Latex\xslt") ) );
                				
			foreach ( FileInfo f in xsltSource.GetFiles( "*.xslt" ) )
			{
				string fname = Path.Combine( workspace.ResourceDirectory, f.Name );
				f.CopyTo( fname, true );
				File.SetAttributes( fname, FileAttributes.Normal );
			}
#else
		
			EmbeddedResources.WriteEmbeddedResources(
				this.GetType().Module.Assembly,
				"NDoc.Documenter.Latex.xslt",
				workspace.ResourceDirectory );
#endif

			string xmlBuffer = MakeXml(project);

			// Create the output directory if it doesn't exist...

			if (!Directory.Exists(LatexConfig.OutputDirectory))
			{
				Directory.CreateDirectory(LatexConfig.OutputDirectory);
			}
			else
			{
				// Delete temp files in the output directory.
				
				OnDocBuildingStep(5, "Deleting temp files from last time...");

				foreach (string s in c_TempFileExtensions)
				{
					File.Delete(LatexConfig.TexFileBaseName + s);
				}				
			}
						
			OnDocBuildingStep(10, "Scanning document text...");

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlBuffer);
			MakeTextTeXCompatible(doc);
			
			WriteTeXDocument(workspace, doc);
			CompileTexDocument();
			workspace.RemoveResourceDirectory();
		}

		
		private string WorkingPath
		{ 
			get
			{ 
				if ( Path.IsPathRooted( LatexConfig.OutputDirectory ) )
					return LatexConfig.OutputDirectory; 

				return Path.GetFullPath( LatexConfig.OutputDirectory );
			} 
		}

		/// <summary>
		/// Translates all text contained in the node (and it's children)
		/// into LaTeX compatable text.
		/// </summary>
		/// <param name="node"></param>
		private void MakeTextTeXCompatible(XmlNode node)
		{
			if (node == null)
			{
				return;
			}

			if (node.NodeType == XmlNodeType.Text)
			{	
				StringBuilder builder = new 
					StringBuilder(node.Value.Length + (node.Value.Length / 2));

				for (int i = 0; i < node.Value.Length; i++)
				{
					char c;
					
					c = node.Value[i];
					
					if 
						(Array.IndexOf(LatexDocumenter.c_LatexEscapeChars, c) >= 0)
					{
						// Replace char with LaTeX escape sequence.

						builder.Append('\\').Append(c);
					}
					else
					{
						builder.Append(c);
					}				
				}

				node.Value = builder.ToString();
			}

			// Recursively change all the text in the node's children

			if (node.HasChildNodes)
			{
				foreach (XmlNode n in node.ChildNodes)
				{
					MakeTextTeXCompatible(n);
				}
			}

			// Change all the text in the attributes as well...

			if (node.Attributes != null)
			{
				foreach (XmlAttribute n in node.Attributes)
				{
					MakeTextTeXCompatible(n);
				}
			}
		}

		/// <summary>
		/// Returns the path to the output file.
		/// </summary>
		/// <remarks>
		/// If a PDF or DVI file with the same name as the TeX file exists 
		/// the path to that file is returned.
		/// </remarks>
		public override string MainOutputFile
		{
			get
			{
				int i;
				String s, retval;

				retval = "";

				i = 0;

				foreach (string ext in c_KnownLatexOuputExtensions)
				{
					s = Path.Combine(
						LatexConfig.OutputDirectory, 
						LatexConfig.TexFileBaseName 
						+ ext);

					if (File.Exists(s))
					{
						retval = s;

						break;
					}

					i++;
				}

				if (retval == "")
				{
					retval = LatexConfig.TexFileFullPath;
				}

				return retval;
			}
		}

		/// <summary>
		/// Calls the LaTeX processor to compile the TeX file into a DVI or PDF.
		/// </summary>
		private void CompileTexDocument()
		{
			Process process;
			ProcessStartInfo startInfo;

			if (LatexConfig.LatexCompiler == "")
			{				
				return;
			}			
			
			startInfo = new ProcessStartInfo(LatexConfig.LatexCompiler, 
				LatexConfig.TextFileFullName);

			startInfo.WorkingDirectory = LatexConfig.OutputDirectory;
			
			// Run LaTeX twice to resolve references.

			for (int i = 0; i < 2; i++)
			{
				process = Process.Start(startInfo);

				OnDocBuildingStep(40 + (i * 30), "Compiling TeX file via " + LatexConfig.LatexCompiler + "...");

				if (process == null)
				{
					throw new DocumenterException("Unable to start the LaTeX compiler: ("
						+ LatexConfig.LatexCompiler + ")");
				}

				process.WaitForExit();
			}
		}

		/// <summary>
		/// Uses XSLT to transform the current document into LaTeX source.
		/// </summary>
		private void WriteTeXDocument( Workspace workspace, XmlDocument document)
		{			
			XmlWriter writer;
			XsltArgumentList args;
			XslTransform transform;

			OnDocBuildingStep(0, "Loading XSLT files...");

			transform = new XslTransform();			
			transform.Load(Path.Combine(workspace.ResourceDirectory, "document.xslt"));

			args = new XsltArgumentList();
			
			writer = new XmlTextWriter(LatexConfig.TexFileFullPath, 
				new UTF8Encoding( false ) );
							
			OnDocBuildingStep(20, "Building TeX file...");

#if(NET_1_0)
				//Use overload that is obsolete in v1.1
			transform.Transform(document, args, writer);
#else
			//Use new overload so we don't get obsolete warnings - clean compile :)
			transform.Transform(document, args, writer, null);
#endif

			writer.Close();
		}
	}
} 


// JavaDocDocumenter.cs - a JavaDoc-like documenter
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
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Reflection;
using System.Collections;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.JavaDoc
{
	/// <summary>The JavaDoc documenter.</summary>
	public class JavaDocDocumenter : BaseReflectionDocumenter
	{
		/// <summary>Initializes a new instance of the JavaDocDocumenter class.</summary>
		public JavaDocDocumenter( JavaDocDocumenterConfig config ) : base( config )
		{
		}

		private Workspace workspace = null;

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override string MainOutputFile 
		{ 
			get 
			{
				return Path.Combine(this.WorkingPath, 
					"overview-summary.html");
			} 
		}

		string tempFileName;
		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override void Build(Project project)
		{
			this.workspace = new JavaDocWorkspace( this.WorkingPath );
			workspace.Clean();
			workspace.Prepare();

			workspace.AddResourceDirectory( "xslt" );
			workspace.AddResourceDirectory( "css" );

// Define this when you want to edit the stylesheets
// without having to shutdown the application to rebuild.
#if NO_RESOURCES
			// copy all of the xslt source files into the workspace
			DirectoryInfo xsltSource = new DirectoryInfo( Path.GetFullPath(Path.Combine(
				System.Windows.Forms.Application.StartupPath, @"..\..\..\Documenter\JavaDoc\xslt") ) );
                				
			foreach ( FileInfo f in xsltSource.GetFiles( "*.xslt" ) )
			{
				string fname = Path.Combine( Path.Combine( workspace.ResourceDirectory, "xslt" ), f.Name );
				f.CopyTo( fname, true );
				File.SetAttributes( fname, FileAttributes.Normal );
			}

			DirectoryInfo cssSource = new DirectoryInfo( Path.GetFullPath(Path.Combine(
				System.Windows.Forms.Application.StartupPath, @"..\..\..\Documenter\JavaDoc\css") ) );
                				
			foreach ( FileInfo f in cssSource.GetFiles( "*.css" ) )
			{
				string cssname = Path.Combine( Path.Combine( workspace.ResourceDirectory, "css" ), f.Name );
				f.CopyTo( cssname, true );
				File.SetAttributes( cssname, FileAttributes.Normal );
			}

#else
				EmbeddedResources.WriteEmbeddedResources(
					this.GetType().Module.Assembly,
					"NDoc.Documenter.JavaDoc.css",
					Path.Combine( this.workspace.ResourceDirectory, "css" ) );

				EmbeddedResources.WriteEmbeddedResources(
					this.GetType().Module.Assembly,
					"NDoc.Documenter.JavaDoc.xslt",
					Path.Combine( this.workspace.ResourceDirectory, "xslt") );
#endif

			string outcss = Path.Combine(MyConfig.OutputDirectory, "JavaDoc.css");
			File.Copy(Path.Combine(workspace.ResourceDirectory, @"css\JavaDoc.css"), outcss, true);
			File.SetAttributes(outcss, FileAttributes.Archive);

			try
			{
				// determine temp file name
				tempFileName = Path.GetTempFileName();
				// Let the Documenter base class do it's thing.
				MakeXmlFile(project, tempFileName);

				WriteOverviewSummary();
				WriteNamespaceSummaries();
			}
			finally
			{
				if (tempFileName != null && File.Exists(tempFileName)) 
				{
					File.Delete(tempFileName);
				}
				workspace.RemoveResourceDirectory();
			}

		}

		private string WorkingPath
		{ 
			get
			{ 
				if ( Path.IsPathRooted( MyConfig.OutputDirectory ) )
					return MyConfig.OutputDirectory; 

				return Path.GetFullPath( MyConfig.OutputDirectory );
			} 
		}

		private JavaDocDocumenterConfig MyConfig
		{
			get { return (JavaDocDocumenterConfig)Config; }
		}

		private void TransformAndWriteResult(
			string transformFilename,
			XsltArgumentList args,
			string resultDirectory,
			string resultFilename)
		{
#if DEBUG
			int start = Environment.TickCount;
#endif
			XslTransform transform = new XslTransform();
			transform.Load(Path.Combine(this.workspace.ResourceDirectory, @"xslt\" + transformFilename));

			if (args == null)
			{
				args = new XsltArgumentList();
			}

			string pathToRoot = "";

			if (resultDirectory != null)
			{
				string[] directories = resultDirectory.Split('\\');
				int count = directories.Length;

				while (count-- > 0)
				{
					pathToRoot += "../";
				}
			}

			args.AddParam("global-path-to-root", String.Empty, pathToRoot);

			if (resultDirectory != null)
			{
				resultFilename = Path.Combine(resultDirectory, resultFilename);
			}

			string resultPath = Path.Combine(MyConfig.OutputDirectory, resultFilename);
			string resultPathDirectory = Path.GetDirectoryName(resultPath);

			if (!Directory.Exists(resultPathDirectory))
			{
				Directory.CreateDirectory(resultPathDirectory);
			}

			TextWriter writer = new StreamWriter(resultPath);
			XPathDocument doc = GetCachedXPathDocument();

#if(NET_1_0)
				//Use overload that is obsolete in v1.1
			transform.Transform(doc, args, writer);
#else
			//Use new overload so we don't get obsolete warnings - clean compile :)
			transform.Transform(doc, args, writer, null);
#endif

			writer.Close();

#if DEBUG
			Trace.WriteLine("Making " + transformFilename + " Html: " + ((Environment.TickCount - start)).ToString() + " ms.");
#endif
		}

		private XPathDocument cachedXPathDocument = null;

		/// <summary>
		/// Gets the XPathDocument, but caches it and returns the cached value.
		/// </summary>
		/// <remarks>
		/// <c>GetXPathDocument</c> can be very slow for large Assemblies, so this is 
		/// designed to speed things up.  As long as the XPathDocument does not ever get 
		/// changed internally by other transforms, this should just speed up the performance 
		/// of the application.
		/// </remarks>
		/// <returns></returns>
		protected XPathDocument GetCachedXPathDocument()
		{
			if(cachedXPathDocument == null)
			{
				Stream tempFile=null;
				try
				{
					tempFile=File.Open(tempFileName,FileMode.Open,FileAccess.Read);
					cachedXPathDocument = new XPathDocument(tempFile);
				}
				finally
				{
					if (tempFile!=null) tempFile.Close();
				}
			}
			return cachedXPathDocument;
		}

		private void WriteOverviewSummary()
		{
			XsltArgumentList args = new XsltArgumentList();
			string title = MyConfig.Title;
			if (title == null) title = string.Empty;
			args.AddParam("global-title", String.Empty, title);

			TransformAndWriteResult(
				"overview-summary.xslt",
				args,
				null,
				"overview-summary.html");
		}

		private void WriteNamespaceSummaries()
		{
			XmlDocument doc = new XmlDocument();
			Stream tempFile=null;
			try
			{
				tempFile=File.Open(tempFileName,FileMode.Open,FileAccess.Read);
				doc.Load(tempFile);
			}
			finally
			{
				if (tempFile!=null) tempFile.Close();
			}

			XmlNodeList namespaceNodes = doc.SelectNodes("/ndoc/assembly/module/namespace");

			foreach (XmlElement namespaceElement in namespaceNodes)
			{
				if (namespaceElement.ChildNodes.Count > 0)
				{
					string name = namespaceElement.GetAttribute("name");
					
					WriteNamespaceSummary(name);
					WriteTypes(namespaceElement);
				}
			}
		}

		private void WriteNamespaceSummary(string name)
		{
			XsltArgumentList args = new XsltArgumentList();
			args.AddParam("global-namespace-name", String.Empty, name);

			TransformAndWriteResult(
				"namespace-summary.xslt",
				args,
				name.Replace('.', '\\'),
				"namespace-summary.html");
		}

		private void WriteTypes(XmlElement namespaceElement)
		{
			XmlNodeList typeNodes = namespaceElement.SelectNodes("interface|class|structure");

			foreach (XmlElement typeElement in typeNodes)
			{
				WriteType(namespaceElement, typeElement);
			}
		}

		private void WriteType(XmlElement namespaceElement, XmlElement typeElement)
		{
			string id = typeElement.GetAttribute("id");

			XsltArgumentList args = new XsltArgumentList();
			args.AddParam("global-type-id", String.Empty, id);

			string namespaceName = namespaceElement.GetAttribute("name");
			string name = typeElement.GetAttribute("name");

			TransformAndWriteResult(
				"type.xslt",
				args,
				namespaceName.Replace('.', '\\'),
				name + ".html");
		}
	}
}

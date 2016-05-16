using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.Intellisense
{
	/// <summary>
	/// 
	/// </summary>
	public class IntellisenseDocumenter : BaseReflectionDocumenter
	{
		private XPathDocument xpathDocument;
		private StyleSheetCollection stylesheets;
		//this encoding is used for all generated output...
		private static readonly UTF8Encoding encoding = new UTF8Encoding(false);

		/// <summary>
		/// Creates a new <see cref="IntellisenseDocumenter"/> instance.
		/// </summary>
		public IntellisenseDocumenter( IntellisenseDocumenterConfig config ) : base(config)
		{
		}

		/// <summary>
		/// Views this instance.
		/// </summary>
		public override void View()
		{
			string OutputDirectory = Path.GetFullPath(MyConfig.OutputDirectory);
			if (Directory.Exists(OutputDirectory))
			{
				string args = String.Format("/root,\"{0}\"", OutputDirectory);
				Process.Start("explorer.exe", args);
			}
			else
			{
				throw new FileNotFoundException("Documentation not built.", 
					this.MainOutputFile);
			}
		}

		/// <summary>
		/// Gets the main output file.
		/// </summary>
		/// <value></value>
		public override string MainOutputFile
		{
			get
			{
				return Path.Combine(MyConfig.OutputDirectory, "*.xml");
			}
		}

		private IntellisenseDocumenterConfig MyConfig
		{
			get
			{
				return (IntellisenseDocumenterConfig)Config;
			}
		}
	
		/// <summary>
		/// Builds the specified project.
		/// </summary>
		/// <param name="project">Project.</param>
		public override void Build(Project project)
		{
			OnDocBuildingStep(0, "Initializing...");

			try
			{
				OnDocBuildingStep(10, "Merging XML documentation...");

				// Will hold the name of the file name containing the XML doc
				string tempFileName = null;

				try 
				{
					// determine temp file name
					tempFileName = Path.GetTempFileName();
					// Let the Documenter base class do it's thing.
					MakeXmlFile(project, tempFileName);

					// Load the XML documentation into XPATH doc.
					using (FileStream tempFile = File.Open(tempFileName, FileMode.Open, FileAccess.Read)) 
					{
						xpathDocument = new XPathDocument(tempFile);
					}
				}
				finally
				{
					if (tempFileName != null && File.Exists(tempFileName)) 
					{
						File.Delete(tempFileName);
					}
				}

				OnDocBuildingStep(30, "Loading XSLT files...");

				stylesheets = StyleSheetCollection.LoadStyleSheets(String.Empty);

				OnDocBuildingStep(40, "Generating XML files...");
				MakeXmlForAssemblies();
			}
			catch (Exception ex)
			{
				throw new DocumenterException(ex.Message, ex);
			}
			finally
			{
				xpathDocument = null;
				stylesheets = null;
			}
		}

		private void MakeXmlForAssemblies()
		{
			XPathNavigator nav = xpathDocument.CreateNavigator();
			XPathNodeIterator iterator = nav.Select("/ndoc/assembly");
			while (iterator.MoveNext())
			{
				string assemblyName = iterator.Current.GetAttribute("name", string.Empty);
				MakeXmlForAssembly(assemblyName);
			}
			OnDocBuildingProgress(100);
		}	
	
		private void MakeXmlForAssembly(string assemblyName)
		{
			string fileName = assemblyName + ".xml";
			XsltArgumentList arguments = new XsltArgumentList();
			arguments.AddParam("assembly-name", String.Empty, assemblyName);
			TransformAndWriteResult("assembly", arguments, fileName);
		}	

		private void TransformAndWriteResult(
			string transformName, 
			XsltArgumentList arguments, 
			string filename)
		{
			Trace.WriteLine(filename);
#if DEBUG
			int start = Environment.TickCount;
#endif

			StreamWriter streamWriter = null;

			try
			{
				using (streamWriter = new StreamWriter(
					File.Open(Path.Combine(MyConfig.OutputDirectory, filename), FileMode.Create), encoding))
				{
					XslTransform transform = stylesheets[transformName];

#if (NET_1_0)
					//Use overload that is now obsolete
					transform.Transform(xpathDocument, arguments, streamWriter);
#else 
					//Use new overload so we don't get obsolete warnings - clean compile :)
					transform.Transform(xpathDocument, arguments, streamWriter, null);
#endif
				}
			}
			catch (PathTooLongException e)
			{
				throw new PathTooLongException(e.Message + "\nThe file that NDoc was trying to create had the following name:\n" + Path.Combine(MyConfig.OutputDirectory, filename));
			}

#if DEBUG
			Debug.WriteLine((Environment.TickCount - start).ToString() + " msec.");
#endif
		}

	
	}
}

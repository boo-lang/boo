// HtmlHelp.cs - helper class to create HTML Help compiler files
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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Globalization;

using Microsoft.Win32;

using NDoc.Core;

namespace NDoc.Documenter.Msdn2
{
	/// <summary>HTML Help file utilities.</summary>
	/// <remarks>This class is used by the MsdnHelp documenter
	/// to create the files needed by the HTML Help compiler.</remarks>
	public class HtmlHelp
	{
		private string _directoryName = null;
		private string _projectName = null;
		private string _defaultTopic = null;

		private string _htmlHelpCompiler = null;

		private bool _includeFavorites = false;
		private bool _binaryTOC = false;
		private short _langID=1033;

		private bool _generateTocOnly;

		private StreamWriter streamHtmlHelp = null;

		private ArrayList _tocFiles = new ArrayList();

		private XmlTextWriter tocWriter;

		/// <summary>Initializes a new instance of the HtmlHelp class.</summary>
		/// <param name="directoryName">The directory to write the HTML Help files to.</param>
		/// <param name="projectName">The name of the HTML Help project.</param>
		/// <param name="defaultTopic">The default topic for the compiled HTML Help file.</param>
		/// <param name="generateTocOnly">When true, HtmlHelp only outputs the HHC file and does not compile the CHM.</param>
		public HtmlHelp(
			string directoryName, 
			string projectName, 
			string defaultTopic,
			bool generateTocOnly)
		{
			_directoryName = directoryName;
			_projectName = projectName;
			_defaultTopic = defaultTopic;
			_generateTocOnly = generateTocOnly;
		}

		/// <summary>Gets the directory name containing the HTML Help files.</summary>
		public string DirectoryName
		{
			get { return _directoryName; }
		}

		/// <summary>Gets the HTML Help project name.</summary>
		public string ProjectName
		{
			get { return _projectName; }
		}

		/// <summary>Gets or sets the IncludeFavorites property.</summary>
		/// <remarks>
		/// Setting this to <see langword="true" /> will include the "favorites" 
		/// tab in the compiled HTML Help file.
		/// </remarks>
		public bool IncludeFavorites
		{
			get { return _includeFavorites; }
			set { _includeFavorites = value; }
		}

		/// <summary>Gets or sets the BinaryTOC property.</summary>
		/// <remarks>
		/// Setting this to <see langword="true" /> will force the compiler 
		/// to create a binary TOC in the chm file.
		/// </remarks>
		public bool BinaryTOC
		{
			get { return _binaryTOC; }
			set { _binaryTOC = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public short LangID
		{
			get { return _langID; }
			set { _langID = value; }
		}

		/// <summary>Gets or sets the DefaultTopic property.</summary>
		public string DefaultTopic
		{
			get { return _defaultTopic; }
			set { _defaultTopic = value; }
		}

		/// <summary>Gets the path to the Html Help Compiler.</summary>
		/// <exception cref="PlatformNotSupportedException">NDoc is running on unix.</exception>
		internal string HtmlHelpCompiler
		{
			get
			{
				if ((int) Environment.OSVersion.Platform == 128) 
				{
					throw new PlatformNotSupportedException(
						"The HTML Help Compiler is not supported on unix.");
				}

				if (_htmlHelpCompiler != null && File.Exists(_htmlHelpCompiler))
				{
					return _htmlHelpCompiler;
				}

				//try the default Html Help Workshop installation directory
				_htmlHelpCompiler = Path.Combine(
					Environment.GetFolderPath(
						Environment.SpecialFolder.ProgramFiles),
					@"HTML Help Workshop\hhc.exe");
				if (File.Exists(_htmlHelpCompiler))
				{
					return _htmlHelpCompiler;
				}

				//not in default dir, try to locate it from the registry
				RegistryKey key = Registry.ClassesRoot.OpenSubKey("hhc.file");
				if (key != null)
				{
					key = key.OpenSubKey("DefaultIcon");
					if (key != null)
					{
						object val = key.GetValue(null);
						if (val != null)
						{
							string hhw = (string)val;
							if (hhw.Length > 0)
							{
								hhw = hhw.Split(new Char[] {','})[0];
								hhw = Path.GetDirectoryName(hhw);
								_htmlHelpCompiler = Path.Combine(hhw, "hhc.exe");
							}
						}
					}
				}
				if (File.Exists(_htmlHelpCompiler))
				{
					return _htmlHelpCompiler;
				}

				// we still can't find the compiler, see if a location is stored in the machine settings file
				Settings settings = new Settings( Settings.MachineSettingsFile );
				string path = settings.GetSetting( "compilers", "htmlHelpWorkshopLocation", "" );

				if ( path.Length > 0 )
				{
					_htmlHelpCompiler = Path.Combine(path, "hhc.exe");
					if (File.Exists(_htmlHelpCompiler))
					{
						return _htmlHelpCompiler;
					}
				}
	
				//still not finding the compiler, give up
				throw new DocumenterException(
					"Unable to find the HTML Help Compiler. Please verify that"
					+ " the HTML Help Workshop has been installed.");
			}
		}

		private string GetContentsFilename()
		{
			return (_tocFiles.Count > 0) ? (string)_tocFiles[0] : string.Empty;
		}

		private string GetIndexFilename()
		{
			return _projectName + ".hhk";
		}

		private string GetLogFilename()
		{
			return _projectName + ".log";
		}

		private string GetCompiledHtmlFilename()
		{
			return _projectName + ".chm";
		}

		/// <summary>Gets the path the the HHP file.</summary>
		public string GetPathToProjectFile()
		{
			return Path.Combine(_directoryName, _projectName) + ".hhp";
		}

		/// <summary>Gets the path the the HHC file.</summary>
		public string GetPathToContentsFile()
		{
			return Path.Combine(_directoryName, GetContentsFilename());
		}

		/// <summary>Gets the path the the HHK file.</summary>
		public string GetPathToIndexFile()
		{
			return Path.Combine(_directoryName, _projectName) + ".hhk";
		}

		/// <summary>Gets the path the the LOG file.</summary>
		public string GetPathToLogFile()
		{
			return Path.Combine(_directoryName, _projectName) + ".log";
		}

		/// <summary>Gets the path the the CHM file.</summary>
		/// <returns>The path to the CHM file.</returns>
		public string GetPathToCompiledHtmlFile()
		{
			return Path.Combine(_directoryName, _projectName) + ".chm";
		}

		/// <summary>Opens an HTML Help project file for writing.</summary>
		public void OpenProjectFile()
		{
			if (_generateTocOnly) 
				return;

			streamHtmlHelp = new StreamWriter(File.Open(GetPathToProjectFile(), FileMode.Create), System.Text.Encoding.Default);
			streamHtmlHelp.WriteLine("[FILES]");
		}

		/// <summary>Adds a file to the HTML Help project file.</summary>
		/// <param name="filename">The filename to add.</param>
		public void AddFileToProject(string filename)
		{
			if (_generateTocOnly) 
				return;

			streamHtmlHelp.WriteLine(filename);
		}

		/// <summary>Closes the HTML Help project file.</summary>
		public void CloseProjectFile()
		{
			if (_generateTocOnly) 
				return;

			string options;

			if (_includeFavorites)
			{
				options = "0x63520,220";
			}
			else
			{
				options = "0x62520,220";						  
			}

			if (_defaultTopic.Length > 0)
			{
				options += ",0x387e,[86,51,872,558],,,,,,,0";
			}
			else
			{
				options += ",0x383e,[86,51,872,558],,,,,,,0";
			}

			streamHtmlHelp.WriteLine();
			streamHtmlHelp.WriteLine("[OPTIONS]");
			streamHtmlHelp.WriteLine("Title=" + _projectName);
			streamHtmlHelp.WriteLine("Auto Index=Yes");

			if (_binaryTOC)
				streamHtmlHelp.WriteLine("Binary TOC=Yes");
			streamHtmlHelp.WriteLine("Compatibility=1.1 or later");
			streamHtmlHelp.WriteLine("Compiled file=" + GetCompiledHtmlFilename());
			streamHtmlHelp.WriteLine("Default Window=MsdnHelp");
			streamHtmlHelp.WriteLine("Default topic=" + _defaultTopic);
			streamHtmlHelp.WriteLine("Display compile progress=No");
			streamHtmlHelp.WriteLine("Error log file=" + GetLogFilename());
			streamHtmlHelp.WriteLine("Full-text search=Yes");
			streamHtmlHelp.WriteLine("Index file=" + GetIndexFilename());
			CultureInfo ci = new CultureInfo(_langID);
			string LangIDString = "Language=0x" + _langID.ToString("x") + " " + ci.DisplayName;
			streamHtmlHelp.WriteLine(LangIDString);

			foreach( string tocFile in _tocFiles )
			{
				streamHtmlHelp.WriteLine("Contents file=" + tocFile);
			}

			streamHtmlHelp.WriteLine();
			streamHtmlHelp.WriteLine("[WINDOWS]");
			streamHtmlHelp.WriteLine("MsdnHelp=\"" +
				_projectName + " Help\",\"" +
				GetContentsFilename() + "\",\"" +
				GetIndexFilename() + "\",\"" +
				_defaultTopic + "\",\"" +
				_defaultTopic + "\",,,,," +
				options);

			streamHtmlHelp.WriteLine();
			streamHtmlHelp.WriteLine("[INFOTYPES]");

			streamHtmlHelp.Close();
		}

		/// <summary>Opens a HTML Help contents file for writing.</summary>
		public void OpenContentsFile(string tocName, bool isDefault)
		{
			// TODO: we would need a more robust way of maintaining the list
			//       of tocs that have been opened...

			if (tocName == string.Empty)
			{
				tocName = _projectName;
			}

			if (!tocName.EndsWith(".hhc"))
			{
				tocName += ".hhc";
			}

			if (isDefault)
			{
				_tocFiles.Insert(0, tocName);
			}
			else
			{
				_tocFiles.Add( tocName );
			}

			// Create the table of contents writer. This can't use
			// indenting because the HTML Help Compiler doesn't like
			// newlines between the <LI> and <Object> tags.
			tocWriter = new XmlTextWriter(Path.Combine(_directoryName, tocName), System.Text.Encoding.Default );

			// these formatting options cannot be used, because they make the 
			// Html Help Compiler hang.
			//			tocWriter.Formatting = Formatting.Indented;
			//			tocWriter.IndentChar = '\t';
			//			tocWriter.Indentation = 1;

			// We don't call WriteStartDocument because that outputs
			// the XML declaration which the HTML Help Compiler doesn't like.

			tocWriter.WriteComment("This document contains Table of Contents information for the HtmlHelp compiler.");
			tocWriter.WriteStartElement("UL");
		}

		/// <summary>Creates a new "book" in the HTML Help contents file.</summary>
		public void OpenBookInContents()
		{
			tocWriter.WriteStartElement("UL");
		}

		/// <summary>Adds a topic to the contents file.</summary>
		/// <param name="headingName">The name as it should appear in the contents.</param>
		/// <remarks>Adds a topic node with no URL associated with the node into the
		/// table of contents.</remarks>
		public void AddFileToContents(string headingName)
		{
			tocWriter.WriteStartElement("LI");
			tocWriter.WriteStartElement("OBJECT");
			tocWriter.WriteAttributeString("type", "text/sitemap");
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "Name");
			tocWriter.WriteAttributeString("value", headingName.Replace('$', '.'));
			tocWriter.WriteEndElement(); // param
			tocWriter.WriteEndElement(); // OBJECT
			tocWriter.WriteEndElement(); // LI
		}

		/// <summary>Adds a topic to the contents file.</summary>
		/// <param name="headingName">The name as it should appear in the contents.</param>
		/// <remarks>Adds a topic node with no URL associated with the node into the
		/// table of contents.</remarks>
		/// <param name="icon">The image for this entry.</param>
		public void AddFileToContents(string headingName, HtmlHelpIcon icon)
		{
			tocWriter.WriteStartElement("LI");
			tocWriter.WriteStartElement("OBJECT");
			tocWriter.WriteAttributeString("type", "text/sitemap");
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "Name");
			tocWriter.WriteAttributeString("value", headingName.Replace('$', '.'));
			tocWriter.WriteEndElement(); // param
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "ImageNumber");
			tocWriter.WriteAttributeString("value", ((int)icon).ToString());
			tocWriter.WriteEndElement(); // param
			tocWriter.WriteEndElement(); // OBJECT
			tocWriter.WriteEndElement(); // LI
		}

		/// <summary>Adds a file to the contents file.</summary>
		/// <param name="headingName">The name as it should appear in the contents.</param>
		/// <param name="htmlFilename">The filename for this entry.</param>
		public void AddFileToContents(string headingName, string htmlFilename)
		{
			tocWriter.WriteStartElement("LI");
			tocWriter.WriteStartElement("OBJECT");
			tocWriter.WriteAttributeString("type", "text/sitemap");
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "Name");
			tocWriter.WriteAttributeString("value", headingName.Replace('$', '.'));
			tocWriter.WriteEndElement();
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "Local");
			tocWriter.WriteAttributeString("value", htmlFilename);
			tocWriter.WriteEndElement();
			tocWriter.WriteEndElement();
			tocWriter.WriteEndElement();
		}

		/// <summary>Adds a file to the contents file.</summary>
		/// <param name="headingName">The name as it should appear in the contents.</param>
		/// <param name="htmlFilename">The filename for this entry.</param>
		/// <param name="icon">The image for this entry.</param>
		public void AddFileToContents(string headingName, string htmlFilename, HtmlHelpIcon icon)
		{
			tocWriter.WriteStartElement("LI");
			tocWriter.WriteStartElement("OBJECT");
			tocWriter.WriteAttributeString("type", "text/sitemap");
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "Name");
			tocWriter.WriteAttributeString("value", headingName.Replace('$', '.'));
			tocWriter.WriteEndElement();
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "Local");
			tocWriter.WriteAttributeString("value", htmlFilename);
			tocWriter.WriteEndElement();
			tocWriter.WriteStartElement("param");
			tocWriter.WriteAttributeString("name", "ImageNumber");
			tocWriter.WriteAttributeString("value", ((int)icon).ToString());
			tocWriter.WriteEndElement();
			tocWriter.WriteEndElement();
			tocWriter.WriteEndElement();
		}

		/// <summary>Closes the last opened "book" in the contents file.</summary>
		public void CloseBookInContents()
		{
			tocWriter.WriteEndElement();
		}

		/// <summary>Closes the contents file.</summary>
		public void CloseContentsFile()
		{
			tocWriter.WriteEndElement();
			tocWriter.Close();
		}

		/// <summary>Writes an empty index file.</summary>
		/// <remarks>The HTML Help Compiler will complain if this file doesn't exist.</remarks>
		public void WriteEmptyIndexFile()
		{
			if (_generateTocOnly) 
				return;

			// Create an empty index file to avoid compilation errors.

			XmlTextWriter indexWriter = new XmlTextWriter(GetPathToIndexFile(), null);

			// Don't call WriteStartDocument to avoid XML declaration.

			indexWriter.WriteStartElement("HTML");
			indexWriter.WriteStartElement("BODY");
			indexWriter.WriteComment(" http://ndoc.sourceforge.net/ ");
			indexWriter.WriteEndElement();
			indexWriter.WriteEndElement();

			// Don't call WriteEndDocument since we didn't call WriteStartDocument.

			indexWriter.Close();
		}

		/// <summary>Compiles the HTML Help project.</summary>
		public void CompileProject()
		{
			if (_generateTocOnly) 
				return;

			Process helpCompileProcess = new Process();

			try
			{
				try
				{
					string path = GetPathToCompiledHtmlFile();

					if (File.Exists(path))
					{
						File.Delete(path);
					}
				}
				catch (Exception e)
				{
					throw new DocumenterException("The compiled HTML Help file is probably open.", e);
				}

				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = HtmlHelpCompiler;
				processStartInfo.Arguments = "\"" + Path.GetFullPath(GetPathToProjectFile()) + "\"";
				processStartInfo.ErrorDialog = false;
				processStartInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				processStartInfo.UseShellExecute = false;
				processStartInfo.CreateNoWindow = true;
				processStartInfo.RedirectStandardError = false; //no point redirecting as HHC does not use stdErr
				processStartInfo.RedirectStandardOutput = true;

				helpCompileProcess.StartInfo = processStartInfo;

				// Start the help compile and bail if it takes longer than 10 minutes.
				Trace.WriteLine( "Compiling Html Help file" );

				string stdOut = "";

				try
				{
					helpCompileProcess.Start();

					// Read the standard output of the spawned process.
					stdOut = helpCompileProcess.StandardOutput.ReadToEnd();
					// compiler std out includes a bunch of unneccessary line feeds + new lines
					// remplace all the line feed and keep the new lines
					stdOut = stdOut.Replace( "\r", "" );
 				}
				catch (Exception e)
				{
					string msg = String.Format("The HTML Help compiler '{0}' was not found.", HtmlHelpCompiler);
					throw new DocumenterException(msg, e);
				}

				helpCompileProcess.WaitForExit();
				//				if (!helpCompileProcess.WaitForExit(600000))
				//				{
				//					throw new DocumenterException("Compile did not complete after 10 minutes and was aborted");
				//				}

				// Errors return 0 (success or warnings returns 1)
				if (helpCompileProcess.ExitCode == 0)
				{
					string ErrMsg = "The Help compiler reported errors";
						if (!File.Exists(GetPathToCompiledHtmlFile()))
						{
							ErrMsg += " - The CHM file was not been created.";
						}
					throw new DocumenterException(ErrMsg + "\n\n" + stdOut);
				}
				else
				{
					Trace.WriteLine(stdOut);
				}

				Trace.WriteLine( "Html Help compile complete" );
			}
			finally
			{
				helpCompileProcess.Close();
			}
		}
	}

	/// <summary>
	/// HtmlHelp v1 TOC icons
	/// </summary>
	public enum HtmlHelpIcon
	{
		/// <summary>
		/// Contents Book
		/// </summary>
		Book=1,
		/// <summary>
		/// Contents Folder
		/// </summary>
		Folder=5,
		/// <summary>
		/// Page with Question Mark
		/// </summary>
		Question=9,
		/// <summary>
		/// Standard Blank Page
		/// </summary>
		Page=11,
		/// <summary>
		/// World
		/// </summary>
		World=13,
		/// <summary>
		/// World w IE icon
		/// </summary>
		WorldInternetExplorer=15, 
		/// <summary>
		/// Information
		/// </summary>
		Information=17,
		/// <summary>
		/// Shortcut
		/// </summary>
		Shortcut=19,
		/// <summary>
		/// BookPage
		/// </summary>
		BookPage=21,
		/// <summary>
		/// Envelope
		/// </summary>
		Envelope=23,
		/// <summary>
		/// Person
		/// </summary>
		Person=27,
		/// <summary>
		/// Sound
		/// </summary>
		Sound=29,
		/// <summary>
		/// Disc
		/// </summary>
		Disc=31,
		/// <summary>
		/// Video
		/// </summary>
		Video=33,
		/// <summary>
		/// Steps
		/// </summary>
		Steps=35,
		/// <summary>
		/// LightBulb
		/// </summary>
		LightBulb=37,
		/// <summary>
		/// Pencil
		/// </summary>
		Pencil=39,
		/// <summary>
		/// Tool
		/// </summary>
		Tool=41
	}
}

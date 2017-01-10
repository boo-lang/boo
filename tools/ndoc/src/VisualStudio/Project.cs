#region Copyright © 2002 Jean-Claude Manoli [jc@manoli.net]
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */ 
#endregion

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace NDoc.VisualStudio
{
	/// <summary>
	/// Represents a Visual Studio c# project file.
	/// </summary>
	public class Project
	{
		internal Project(Solution solution, Guid id, string name)
		{
			_Solution = solution;
			_ID = id;
			_Name = name;
		}

		private Solution _Solution;

		/// <summary>Gets the solution that contains this project.</summary>
		public Solution Solution
		{
			get { return _Solution; }
		}

		private string _RelativePath;

		/// <summary>Gets or sets the relative path (from the solution 
		/// directory) to the project directory.</summary>
		public string RelativePath
		{
			get { return _RelativePath; }
			set { _RelativePath = value; }
		}

		private Guid _ID;

		/// <summary>Gets the GUID that identifies the project.</summary>
		public Guid ID
		{
			get { return _ID; }
		}

		private string _Name;

		/// <summary>Gets the name of the project.</summary>
		public string Name
		{
			get { return _Name; }
		}

		private XPathDocument _ProjectDocument;
		private XPathNavigator _ProjectNavigator;

		/// <summary>Reads the project file from the specified path.</summary>
		/// <param name="path">The path to the project file.</param>
		public void Read(string path)
		{
			_ProjectDocument = new XPathDocument(path);
			_ProjectNavigator = _ProjectDocument.CreateNavigator();
		}

		/// <summary>Gets a string that represents the type of project.</summary>
		/// <value>"Visual C++" or "C# Local"</value>
		public string ProjectType
		{
			get
			{
				string projectType = "";

				if ((bool)_ProjectNavigator.Evaluate("boolean(VisualStudioProject/@ProjectType='Visual C++')"))
				{
					projectType = "Visual C++";
				}
				else if ((bool)_ProjectNavigator.Evaluate("boolean(VisualStudioProject/CSHARP/@ProjectType='Local')"))
				{
					projectType = "C# Local";
				}
				else if ((bool)_ProjectNavigator.Evaluate("boolean(VisualStudioProject/CSHARP/@ProjectType='Web')"))
				{
					projectType = "C# Web";
				}
				return projectType;
			}
		}

		/// <summary>Gets the name of the assembly this project generates.</summary>
		public string AssemblyName
		{
			get
			{
				return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@AssemblyName)");
			}
		}

		/// <summary>Gets the output type of the project.</summary>
		/// <value>"Library", "Exe", or "WinExe"</value>
		public string OutputType
		{
			get
			{
				return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@OutputType)");
			}
		}

		/// <summary>Gets the filename of the generated assembly.</summary>
		public string OutputFile
		{
			get
			{
				string extension = "";

				switch (OutputType)
				{
					case "Library":
						extension = ".dll";
						break;
					case "Exe":
						extension = ".exe";
						break;
					case "WinExe":
						extension = ".exe";
						break;
				}

				return AssemblyName + extension;
			}
		}

		/// <summary>Gets the default namespace for the project.</summary>
		public string RootNamespace
		{
			get
			{
				return (string)_ProjectNavigator.Evaluate("string(/VisualStudioProject/CSHARP/Build/Settings/@RootNamespace)");
			}
		}

		/// <summary>Gets the configuration with the specified name.</summary>
		/// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
		/// <returns>A ProjectConfig object.</returns>
		public ProjectConfig GetConfiguration(string configName)
		{
			XPathNavigator navigator = null;

			XPathNodeIterator nodes = 
				_ProjectNavigator.Select(
				String.Format(
				"/VisualStudioProject/CSHARP/Build/Settings/Config[@Name='{0}']", 
				configName));

			if (nodes.MoveNext())
			{
				navigator = nodes.Current;
			}
		
			return new ProjectConfig(navigator);
		}

		/// <summary>Gets the relative path (from the solution directory) to the
		/// assembly this project generates.</summary>
		/// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
		public string GetRelativeOutputPathForConfiguration(string configName)
		{
			return Path.Combine(
				Path.Combine(RelativePath, GetConfiguration(configName).OutputPath), 
				OutputFile);
		}

		/// <summary>Gets the relative path (from the solution directory) to the
		/// XML documentation this project generates.</summary>
		/// <param name="configName">A valid configuration name, usually "Debug" or "Release".</param>
		public string GetRelativePathToDocumentationFile(string configName)
		{
			string path = null;

			string documentationFile = GetConfiguration(configName).DocumentationFile;

			if (documentationFile != null && documentationFile.Length > 0)
			{
				path = Path.Combine(RelativePath, documentationFile);
			}

			return path;
		}

	}
}

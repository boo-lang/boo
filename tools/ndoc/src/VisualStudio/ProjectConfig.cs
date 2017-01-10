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
	/// ProjectConfig settings for Visual Studio C# projects.
	/// </summary>
	public class ProjectConfig
	{
		private XPathNavigator _Navigator;

		internal ProjectConfig(XPathNavigator navigator)
		{
			_Navigator = navigator.Clone();
		}

		/// <summary>Gets the name of the configuration.</summary>
		/// <remarks>This is usually "Debug" or "Release".</remarks>
		public string Name
		{
			get
			{
				return (string)_Navigator.Evaluate("string(@Name)");
			}
		}

		/// <summary>Gets the location of the output files (relative to the 
		/// project directory) for this project's configuration.</summary>
		public string OutputPath
		{
			get
			{
				return (string)_Navigator.Evaluate("string(@OutputPath)");
			}
		}

		/// <summary>Gets the name of the file (relative to the project 
		/// directory) into which documentation comments will be 
		/// processed.</summary>
		public string DocumentationFile
		{
			get
			{
				return (string)_Navigator.Evaluate("string(@DocumentationFile)");
			}
		}
	}
}

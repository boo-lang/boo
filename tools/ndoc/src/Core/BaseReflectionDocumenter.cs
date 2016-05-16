// BaseReflectionDocumenter.cs - base XML documenter code
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
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace NDoc.Core.Reflection
{
	/// <summary>The base class for documenters which use the <see cref="ReflectionEngine"/> to extract 
	/// documentation from .Net assemblies.</summary>
	abstract public class BaseReflectionDocumenter : BaseDocumenter
	{
		private BaseReflectionDocumenterConfig MyConfig
		{
			get
		{
				return (BaseReflectionDocumenterConfig)Config;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseReflectionDocumenter"/> class.
		/// </summary>
		/// <param name="config">Documenter setting</param>
		protected BaseReflectionDocumenter( IDocumenterConfig config ) : base( config )
		{
		}

		/// <summary>
		/// Writes reflected metadata combined with the /doc comments to the 
		/// specified file.
		/// </summary>
		/// <remarks>
		/// This is performed in a separate <see cref="AppDomain" />.
		/// </remarks>
		protected void MakeXmlFile(Project project, string fileName)
		{
			//if this.rep.UseNDocXmlFile is set, 
			//copy it to the temp file and return.
			string xmlFile = MyConfig.UseNDocXmlFile;
			if (xmlFile.Length > 0)
			{
				Trace.WriteLine("Loading pre-compiled XML information from: " + xmlFile);
				File.Copy(xmlFile, fileName, true);
				return;
			}

            AppDomain appDomain = null;
			try 
			{
				appDomain = AppDomain.CreateDomain("NDocReflection", 
					AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
                appDomain.SetupInformation.ShadowCopyFiles = "true"; //only required for managed c++ assemblies
                ReflectionEngine re = (ReflectionEngine)   
                    appDomain.CreateInstanceAndUnwrap(typeof(ReflectionEngine).Assembly.FullName, 
                    typeof(ReflectionEngine).FullName, false, BindingFlags.Public | BindingFlags.Instance, 
                    null, new object[0], CultureInfo.InvariantCulture, new object[0], 
                    AppDomain.CurrentDomain.Evidence);
				ReflectionEngineParameters rep = new ReflectionEngineParameters(
					project, MyConfig);
				re.MakeXmlFile(rep, fileName);
			} 
			finally 
			{
				if (appDomain != null) AppDomain.Unload(appDomain);
			}
		}

		/// <summary>
		/// Returns reflected metadata combined with the /doc comments.
		/// </summary>
		/// <remarks>This now evidently writes the string in utf-16 format (and 
		/// says so, correctly I suppose, in the xml text) so if you write this string to a file with 
		/// utf-8 encoding it will be unparseable because the file will claim to be utf-16
		/// but will actually be utf-8.</remarks>
		/// <returns>XML string</returns>
		/// <remarks>
		/// This is performed in a separate <see cref="AppDomain" />.
		/// </remarks>
		protected string MakeXml(Project project)
		{
			//if MyConfig.UseNDocXmlFile is set, 
			//load the XmlBuffer from the file and return.
			string xmlFile = MyConfig.UseNDocXmlFile;
			if (xmlFile.Length > 0)
			{
				Trace.WriteLine("Loading pre-compiled XML information from: " + xmlFile);
				using (TextReader reader = new StreamReader(xmlFile, Encoding.UTF8))
				{
					return reader.ReadToEnd();
				}
			}

			AppDomain appDomain = null;

			try 
			{
				appDomain = AppDomain.CreateDomain("NDocReflection", 
					AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
				appDomain.SetupInformation.ShadowCopyFiles = "true"; //only required for managed c++ assemblies
				ReflectionEngine re = (ReflectionEngine)   
                    appDomain.CreateInstanceAndUnwrap(typeof(ReflectionEngine).Assembly.FullName, 
                    typeof(ReflectionEngine).FullName, false, BindingFlags.Public | BindingFlags.Instance, 
                    null, new object[0], CultureInfo.InvariantCulture, new object[0], 
                    AppDomain.CurrentDomain.Evidence);
                ReflectionEngineParameters rep = new ReflectionEngineParameters(
					project, MyConfig);
				return re.MakeXml(rep);
			} 
			finally 
			{
				if (appDomain != null) AppDomain.Unload(appDomain);
			}
		}
	}
}

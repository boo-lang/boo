// XmlDocumenter.cs - an XML documenter
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
using System.Xml;
using System.Xml.Serialization;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.Xml
{
	/// <summary>The XmlDocumenter class.</summary>
	public class XmlDocumenter : BaseReflectionDocumenter
	{
		/// <summary>Initializes a new instance of the XmlDocumenter class.</summary>
		public XmlDocumenter( XmlDocumenterConfig config ) : base( config )
		{
		}

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override string MainOutputFile 
		{ 
			get 
			{
				return ((XmlDocumenterConfig)Config).OutputFile;
			} 
		}

		/// <summary>See IDocumenter.</summary>
		public override void Build(Project project)
		{
			OnDocBuildingStep(0, "Building XML documentation...");

			XmlDocumenterConfig config = (XmlDocumenterConfig)Config;

			string tempFileName = null;
			
			try 
			{
				// Determine temp file name
				tempFileName = Path.GetTempFileName();
				// Let the Documenter base class do it's thing.
				MakeXmlFile(project, tempFileName);

				OnDocBuildingStep(50, "Saving XML documentation...");

				string outputFileName = project.GetFullPath(config.OutputFile);

				string directoryName = Path.GetDirectoryName(outputFileName);

				if (directoryName != null && directoryName.Length > 0)
				{
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
				}

				if (File.Exists(outputFileName)) 
				{
					File.Delete(outputFileName);
				}

				File.Move(tempFileName, outputFileName);

				OnDocBuildingStep(100, "Done.");
			} 
			finally 
			{
				if (tempFileName != null && File.Exists(tempFileName)) 
				{
					File.Delete(tempFileName);
				}
			}
		}
	}
}

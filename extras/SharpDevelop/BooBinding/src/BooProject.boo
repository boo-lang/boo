#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import System
import System.IO
import System.Collections
import System.Diagnostics
import System.Reflection
import System.Xml
import ICSharpCode.Core.Services
import ICSharpCode.SharpDevelop.Services
import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Internal.Templates

// Describes a Boo Project and its compilation options.
class BooProject(AbstractProject):
	override ProjectType:
		get:
			return BooLanguageBinding.LanguageName
	
	override def CreateConfiguration() as IConfiguration:
		return BooCompilerParameters()
	
	def constructor(info as ProjectCreateInformation, projectOptions as XmlElement):
		parserService as IParserService = ServiceManager.Services.GetService(typeof(IParserService))
		booDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
		parserService.AddReferenceToCompletionLookup(self, ProjectReference(ReferenceType.Assembly, Path.Combine(booDir, "Boo.Lang.dll")))
		if info != null:
			Name = info.ProjectName;
			debugConf as BooCompilerParameters = CreateConfiguration("Debug")
			Configurations.Add(debugConf)
			debugConf.IncludeDebugInformation = true
			
			Configurations.Add(CreateConfiguration("Release"))
			
			for parameter as BooCompilerParameters in Configurations:
				parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name
				parameter.OutputAssembly = info.ProjectName
				
				if projectOptions != null:
					target = projectOptions.GetAttribute("Target")
					pauseConsoleOutput = projectOptions.GetAttribute("PauseConsoleOutput")
					duckTypingByDefault = projectOptions.GetAttribute("duckTypingByDefault")
					
					if target != null and target != "":
						parameter.CompileTarget = Enum.Parse(typeof(CompileTarget), target)
					if pauseConsoleOutput != null and pauseConsoleOutput != "":
						parameter.PauseConsoleOutput = Boolean.Parse(pauseConsoleOutput)
					if duckTypingByDefault != null and duckTypingByDefault != "":
						parameter.DuckTypingByDefault = Boolean.Parse(duckTypingByDefault)
				

namespace BooBinding

//using System;
//using System.IO;
//using System.Collections;
//using System.Diagnostics;
//using System.ComponentModel;
//using System.Xml;
//using ICSharpCode.SharpDevelop.Internal.Project;
//using ICSharpCode.SharpDevelop.Internal.Templates;

class BooProject(AbstractProject):
"""Describes a Boo Project and it compilation options.
"""
	public override ProjectType as string:
		get:
			return BooLanguageBinding.LanguageName
			
	public override def CreateConfiguration() as IConfiguration:
		return BooCompilerParameters()
		
	public constructor(info as ProjectCreateInformation, projectOptions as XmlElement):
		return unless info
		
		Name = info.ProjectName
		Configurations.Add(CreateConfiguration("Debug"))
		Configurations.Add(CreateConfiguration("Release"))
	
		for parameter in Configurations:
			parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name
			parameter.OutputAssembly = Name
			if projectOptions:
				if projectOptions.Attributes["Target"]:
					parameter.CompilerTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), getAttr(projectOptions, "Target")) 
				if projectOptions.Attributes["PauseConsoleAttribute"]:
					parameter.PauseConsoleOutput = Boolean.Parse(getAttr(projectOptions, "PauseConsoleAttribute"))

	protected def getAttr(projectOptions as XmlElement, attr as String) as String:
		return projectOptions.Attributes[attr].InnerText
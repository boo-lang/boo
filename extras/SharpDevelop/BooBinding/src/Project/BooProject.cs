using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace BooBinding
{
	/// Describes a Boo Project and it compilation options.
	class BooProject : AbstractProject
	{
		public override string ProjectType
		{
			get { return BooLanguageBinding.LanguageName; }
		}
				
		public override IConfiguration CreateConfiguration()
		{
			return new BooCompilerParameters();
		}
						
		public BooProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) 
			{
				Name = info.ProjectName;
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (BooCompilerParameters parameter in Configurations) 
				{
					parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.OutputAssembly  = Name;
					//}
					//XmlElement el = info.ProjectTemplate.ProjectOptions; -- moved above foreach loop
					//para variable renamed parameter
					//CSharpCompilerParameters para = ActiveConfiguration;?? - removed as nolonger needed
					if (projectOptions != null) 
					{
						if (projectOptions.Attributes["Target"] != null) 
						{
							parameter.CompileTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), projectOptions.Attributes["Target"].InnerText);
						}
						if (projectOptions.Attributes["PauseConsoleOutput"] != null) 
						{
							parameter.PauseConsoleOutput = Boolean.Parse(projectOptions.Attributes["PauseConsoleOutput"].InnerText);
						}
					}
				}
			}
		}

		protected string getAttr(XmlElement projectOptions, string attr)
		{
			return projectOptions.Attributes[attr].InnerText;
		}
	}
}

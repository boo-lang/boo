using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace BooBinding
{
	public class BooLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Boo";
		
		BooBindingCompilerManager   compilerManager  = new BooBindingCompilerManager();
		BooBindingExecutionManager  executionManager = new BooBindingExecutionManager();
		
		public string Language 
		{
			get 
			{
				return LanguageName;
			}
		}
		
		public void Execute(string filename)
		{
			Debug.Assert(executionManager != null);
			executionManager.Execute(filename);
		}
		
		public void Execute(IProject project)
		{
			Debug.Assert(executionManager != null);
			executionManager.Execute(project);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.GetCompiledOutputName(fileName);
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.GetCompiledOutputName(project);
		}
		
		public bool CanCompile(string fileName)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.CanCompile(fileName);
		}
		
		public ICompilerResult CompileFile(string fileName)
		{
			Debug.Assert(compilerManager != null);
			BooCompilerParameters param = new BooCompilerParameters();
			param.OutputAssembly = Path.ChangeExtension(fileName, ".exe");
			return compilerManager.CompileFile(fileName, param);
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.CompileProject(project);
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			return CompileProject(project);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new BooProject(info, projectOptions);
		}
	}
}

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace BooBinding
{
	/// <summary>
	/// This class controls the compilation of Boo files and Boo projects
	/// </summary>
	public class BooBindingCompilerManager
	{	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			BooProject p = (BooProject)project;
			BooCompilerParameters compilerparameters = (BooCompilerParameters)p.ActiveConfiguration;
			string exe  = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			return exe;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".BOO";
		}
		
		public ICompilerResult CompileFile(string filename, BooCompilerParameters compilerparameters)
		{
			/*
			string output = "";
			string error  = "";
			string exe = Path.ChangeExtension(filename, ".exe");
			if (compilerparameters.OutputAssembly != null && compilerparameters.OutputAssembly.Length > 0) {
				exe = compilerparameters.OutputAssembly;
			}
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);

			writer.WriteLine("\"/out:" + exe + '"');
			
			writer.WriteLine("/nologo");
			writer.WriteLine("/utf8output");
			writer.WriteLine("/w:" + compilerparameters.WarningLevel);
			writer.WriteLine("/nowarn:" + compilerparameters.NoWarnings);
			
			if (compilerparameters.Debugmode) {
				writer.WriteLine("/debug:+");
				writer.WriteLine("/debug:full");
				writer.WriteLine("/d:DEBUG");
			}
			
			if (compilerparameters.TreatWarningsAsErrors) {
				writer.WriteLine("/warnaserror+");
			}
			
			if (compilerparameters.Optimize) {
				writer.WriteLine("/o");
			}
			
			if (compilerparameters.UnsafeCode) {
				writer.WriteLine("/unsafe");
			}
			
			if (compilerparameters.DefineSymbols.Length > 0) {
				writer.WriteLine("/define:" + '"' + compilerparameters.DefineSymbols + '"');
			}
			
			switch (compilerparameters.CompileTarget) {
				case CompileTarget.Exe:
					writer.WriteLine("/target:exe");
					break;
				case CompileTarget.WinExe:
					writer.WriteLine("/target:winexe");
					break;
				case CompileTarget.Library:
					writer.WriteLine("/target:library");
					break;
				case CompileTarget.Module:
					writer.WriteLine("/target:module");
					break;
			}
			
			writer.WriteLine('"' + filename + '"');
			
			TempFileCollection  tf = new TempFileCollection ();
			
			if (compilerparameters.GenerateXmlDocumentation) {
				writer.WriteLine("\"/doc:" + Path.ChangeExtension(exe, ".xml") + '"');
			}	
			
			writer.Close();
			
			string compilerName = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? GetCompilerName() : "mcs";
			string outstr =  compilerName + " \"@" + responseFileName + "\"";
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
			*/

			return null;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			/*
			BooProject p = (BooProject)project;
			BooCompilerParameters compilerparameters = (BooCompilerParameters)p.ActiveConfiguration;
			
			string exe              = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			string responseFileName = Path.GetTempFileName();
			StreamWriter writer = new StreamWriter(responseFileName);
			
			string optionString = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? "/" : "-";
			
			if (compilerparameters.CsharpCompiler == CsharpCompiler.Csc) {
				writer.WriteLine("\"/out:" + exe + '"');
				
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				
				foreach (ProjectReference lib in p.ProjectReferences) {
					string fileName = lib.GetReferencedFileName(p);
					writer.WriteLine("\"/r:" + fileName + "\"");
				}
				
				writer.WriteLine("/nologo");
				writer.WriteLine("/utf8output");
				writer.WriteLine("/w:" + compilerparameters.WarningLevel);;
				
				if (compilerparameters.Debugmode) {
					writer.WriteLine("/debug:+");
					writer.WriteLine("/debug:full");
					writer.WriteLine("/d:DEBUG");
				}
				
				if (compilerparameters.Optimize) {
					writer.WriteLine("/o");
				}
				
				if (compilerparameters.Win32Icon != null && compilerparameters.Win32Icon.Length > 0 && File.Exists(compilerparameters.Win32Icon)) {
					writer.WriteLine("\"/win32icon:" + compilerparameters.Win32Icon + "\"");
				}
				
				if (compilerparameters.UnsafeCode) {
					writer.WriteLine("/unsafe");
				}
				
				if (compilerparameters.DefineSymbols.Length > 0) {
					writer.WriteLine("/define:" + '"' + compilerparameters.DefineSymbols + '"');
				}
				
				if (compilerparameters.MainClass != null && compilerparameters.MainClass.Length > 0) {
					writer.WriteLine("/main:" + compilerparameters.MainClass);
				}
				
				switch (compilerparameters.CompileTarget) {
					case CompileTarget.Exe:
						writer.WriteLine("/t:exe");
						break;
					case CompileTarget.WinExe:
						writer.WriteLine("/t:winexe");
						break;
					case CompileTarget.Library:
						writer.WriteLine("/t:library");
						break;
				}
				
				foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.Subtype != Subtype.Directory) {
						switch (finfo.BuildAction) {
							case BuildAction.Compile:
								Console.Error.WriteLine(finfo.Name);
								writer.WriteLine('"' + finfo.Name + '"');
								break;
							case BuildAction.EmbedAsResource:
								writer.WriteLine("\"/res:" + finfo.Name + "\"");
								break;
						}
					}
				}
				
				if (compilerparameters.GenerateXmlDocumentation) {
					writer.WriteLine("\"/doc:" + Path.ChangeExtension(exe, ".xml") + '"');
				}
			} 
			else {
				writer.WriteLine("-o " + exe);
				
				if (compilerparameters.UnsafeCode) {
					writer.WriteLine("--unsafe");
				}
				
				writer.WriteLine("--wlevel " + compilerparameters.WarningLevel);
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				
				foreach (ProjectReference lib in p.ProjectReferences) {
					string fileName = lib.GetReferencedFileName(p);
					writer.WriteLine("-r:" + fileName );
				}
				
				switch (compilerparameters.CompileTarget) {
					case CompileTarget.Exe:
						writer.WriteLine("--target exe");
						break;
					case CompileTarget.WinExe:
						writer.WriteLine("--target winexe");
						break;
					case CompileTarget.Library:
						writer.WriteLine("--target library");
						break;
				}
				foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.Subtype != Subtype.Directory) {
						switch (finfo.BuildAction) {
							case BuildAction.Compile:
								writer.WriteLine('"' + finfo.Name + '"');
								break;
							
							case BuildAction.EmbedAsResource:
								writer.WriteLine("--linkres " + finfo.Name);
								break;
						}
					}
				}			
			}
			writer.Close();
			
			string output = String.Empty;
			string error  = String.Empty; 
			
			string compilerName = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? GetCompilerName() : System.Environment.GetEnvironmentVariable("ComSpec") + " /c mcs";
			string outstr = compilerName + " @" + responseFileName;
			TempFileCollection tf = new TempFileCollection();
			
			Executor.ExecWaitWithCapture(outstr,  tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			project.CopyReferencesToOutputPath(false);
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
			*/

			return null;
		}
		
		string GetCompilerName()
		{
//			return fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()) + 
//			       "csc.exe";

			return "";
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			/*
			StringBuilder compilerOutput = new StringBuilder();
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
			// we have 2 formats for the error output the csc gives :
			Regex normalError  = new Regex(@"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
			Regex generalError = new Regex(@"(?<error>.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
			
			while (true) {
				string curLine = sr.ReadLine();
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
				curLine = curLine.Trim();
				if (curLine.Length == 0) {
					continue;
				}
				
				CompilerError error = new CompilerError();
				
				// try to match standard errors
				Match match = normalError.Match(curLine);
				if (match.Success) {
					error.Column      = Int32.Parse(match.Result("${column}"));
					error.Line        = Int32.Parse(match.Result("${line}"));
					error.FileName    = Path.GetFullPath(match.Result("${file}"));
					error.IsWarning   = match.Result("${error}") == "warning"; 
					error.ErrorNumber = match.Result("${number}");
					error.ErrorText   = match.Result("${message}");
				} else {
					match = generalError.Match(curLine); // try to match general csc errors
					if (match.Success) {
						error.IsWarning   = match.Result("${error}") == "warning"; 
						error.ErrorNumber = match.Result("${number}");
						error.ErrorText   = match.Result("${message}");
					} else { // give up and skip the line
						continue;
//						error.IsWarning = false;
//						error.ErrorText = curLine;
					}
				}
				
				cr.Errors.Add(error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
			*/

			return null;
		}
	}
}

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
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.Core.Services;

namespace BooBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class BooBindingExecutionManager
	{
		public void Execute(string filename)
		{
			/*
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = false;
			try {
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + exe + "\"\n(.NET bug? Try restaring SD or manual start)");
			}
			*/
		}
		
		public void Execute(IProject project)
		{
			/*
			CSharpCompilerParameters parameters = (CSharpCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((CSharpCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			string exe = ((CSharpCompilerParameters)project.ActiveConfiguration).OutputAssembly + ".exe";
			string args = ((CSharpCompilerParameters)project.ActiveConfiguration).CommandLineParameters;
			
			ProcessStartInfo psi;
			if (parameters.ExecuteScript != null && parameters.ExecuteScript.Length > 0) {
				Console.WriteLine("EXECUTE SCRIPT!!!!!!");
			psi = new ProcessStartInfo("\"" + parameters.ExecuteScript + "\"");
			} else {
				string runtimeStarter = String.Empty;
				
				switch (parameters.NetRuntime) {
					case NetRuntime.Mono:
						runtimeStarter = "mono ";
						break;
					case NetRuntime.MonoInterpreter:
						runtimeStarter = "mint ";
						break;
				}
				
				if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput) {
					psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + runtimeStarter + "\"" + directory + exe + "\" " + args +  " & pause");
				} else {
					psi = new ProcessStartInfo(runtimeStarter + "\"" + directory + exe + "\"");
					psi.Arguments = args;
				}
			}
			
			try {
				psi.WorkingDirectory = Path.GetDirectoryName(directory);
				psi.UseShellExecute  =  false;
				
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + directory + exe + "\"");
			}
			*/
		}
	}
}

#region license
// Copyright (c) 2004, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding

import System
import System.Collections
import System.IO
import System.Diagnostics
import System.Text
import System.Text.RegularExpressions
import System.CodeDom.Compiler

import ICSharpCode.Core.Services

import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.SharpDevelop.Services

class BooBindingCompilerManager:
	static _normalError  = /(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<number>[\d\w]+):\s+(?<message>.*)/
	static _generalError = /(?<error>.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)/
	
	def GetCompiledOutputName(fileName as string) as string:
		return Path.ChangeExtension(fileName, ".exe")
	
	def GetCompiledOutputName(project as IProject) as string:
		p as BooProject = project;
		compilerparameters as BooCompilerParameters = p.ActiveConfiguration
		
		fileUtilityService as FileUtilityService = ServiceManager.Services.GetService(typeof(FileUtilityService))
		exe = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly
		if (compilerparameters.CompileTarget == CompileTarget.Library):
			return exe + ".dll"
		else:
			return exe + ".exe"
	
	def CanCompile(fileName as string) as bool:
		return Path.GetExtension(fileName).ToLower() == ".boo";
	
	static _compilerNotFound = "Boo compiler not found.\nYou have to specify the path where booc.exe is in the project options!\n"
	
	private def MakeError(text as string):
		cr = CompilerResults(TempFileCollection())
		cr.Errors.Add(CompilerError(ErrorText: text))
		return DefaultCompilerResult(cr, text)
	
	private def Compile(compilerparameters as BooCompilerParameters, fileNames as (string), outputFile as string, p as IProject) as ICompilerResult:
		fileUtilityService as FileUtilityService = ServiceManager.Services.GetService(typeof(FileUtilityService))
		messageService as IMessageService = ServiceManager.Services.GetService(typeof(IMessageService))
		
		if compilerparameters.BooPath == "":
			messageService.ShowError(_compilerNotFound)
			return MakeError(_compilerNotFound)
		booDir = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.BooPath)
		compilerName = booDir + "booc.exe"
		
		if not File.Exists(compilerName):
			messageService.ShowError(_compilerNotFound)
			return MakeError(_compilerNotFound)
		
		cmd = StringBuilder()
		cmd.Append("\"${compilerName}\"")
		
		cmd.Append(" -o:\"${outputFile}\"")
		
		cmd.Append(" -vv") if compilerparameters.Verbose
		
		cmd.Append(" -t:")
		if (compilerparameters.CompileTarget == CompileTarget.WinExe):
			cmd.Append("winexe")
		else:
			if (compilerparameters.CompileTarget == CompileTarget.Library):
				cmd.Append("library");
			else:
				cmd.Append("exe");
		
		// Default references are already added by booc.rsp
		
		if p != null:
			// write references
			for lib as ProjectReference in p.ProjectReferences:
				cmd.Append(" \"-r:${lib.GetReferencedFileName(p)}\"");
			// write embedded resources
			for finfo as ProjectFile in p.ProjectFiles:
				if finfo.Subtype != Subtype.Directory and finfo.BuildAction == BuildAction.EmbedAsResource:
					cmd.Append(" \"-resource:${finfo.Name}\"")
		
		for fileName as string in fileNames:
			cmd.Append(" \"${Path.GetFullPath(fileName)}\"")
		
		outstr = cmd.ToString()
		tf = TempFileCollection()
		output = ""
		error = ""
		Executor.ExecWaitWithCapture(outstr, Path.GetFullPath(compilerparameters.OutputDirectory), tf, output, error)
		
		result = ParseOutput(tf, output)		
		
		outputDirectory = Path.GetDirectoryName(outputFile)
		
		CopyToDirIgnoringErrors(Path.Combine(booDir, "Boo.dll"), outputDirectory)
		for lib as ProjectReference in p.ProjectReferences:
			if lib.LocalCopy:
				CopyToDirIgnoringErrors(
					lib.GetReferencedFileName(p),
					outputDirectory)
		
		File.Delete(output)
		File.Delete(error)
		return result
		
	def CopyToDirIgnoringErrors(fname as string, outputDirectory as string):
		try:
			File.Copy(fname, Path.Combine(outputDirectory, Path.GetFileName(fname)))
		except:
			pass
		
	
	def CompileFile(fileName as string, compilerparameters as BooCompilerParameters) as ICompilerResult:
		compilerparameters.OutputDirectory = Path.GetDirectoryName(fileName)
		compilerparameters.OutputAssembly  = Path.GetFileNameWithoutExtension(fileName)
		
		return Compile(compilerparameters, (fileName,), GetCompiledOutputName(fileName), null)
	
	def CompileProject(project as BooProject) as ICompilerResult:
		compilerparameters as BooCompilerParameters = project.ActiveConfiguration
		
		fileNames = ArrayList()
		
		for	finfo as ProjectFile in project.ProjectFiles:
			if finfo.Subtype != Subtype.Directory:
				if finfo.BuildAction == BuildAction.Compile:
						fileNames.Add(finfo.Name)
		
		exe = GetCompiledOutputName(project)
		if compilerparameters.CompileTarget == CompileTarget.WinExe:
			WriteManifestFile(exe)
		
		return Compile(compilerparameters, fileNames.ToArray(typeof(string)), exe, project)
	
	def WriteManifestFile(fileName as string):
		manifestFile = fileName + ".manifest"
		return if File.Exists(manifestFile)
		using sw = StreamWriter(manifestFile):
			sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>")
			sw.WriteLine("")
			sw.WriteLine("<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\">")
			sw.WriteLine("	<dependency>")
			sw.WriteLine("		<dependentAssembly>")
			sw.WriteLine("			<assemblyIdentity")
			sw.WriteLine("				type=\"win32\"")
			sw.WriteLine("				name=\"Microsoft.Windows.Common-Controls\"")
			sw.WriteLine("				version=\"6.0.0.0\"")
			sw.WriteLine("				processorArchitecture=\"X86\"")
			sw.WriteLine("				publicKeyToken=\"6595b64144ccf1df\"")
			sw.WriteLine("				language=\"*\"")
			sw.WriteLine("			/>")
			sw.WriteLine("		</dependentAssembly>")
			sw.WriteLine("	</dependency>")
			sw.WriteLine("</assembly>")
	
	def ParseOutput(tf as TempFileCollection, file as string) as ICompilerResult:
		compilerOutput = StringBuilder();
		
		using sr = File.OpenText(file):
			cr = CompilerResults(tf)
			
			curLine = sr.ReadLine()
			while curLine != null:
				compilerOutput.Append(curLine)
				compilerOutput.Append("\n")
				curLine = curLine.Trim();
				if curLine.Length == 0:
					curLine = sr.ReadLine()
					continue
				
				error = CompilerError()
				
				// try to match standard errors
				match = _normalError.Match(curLine)
				if match.Success:
					error.Column      = Int32.Parse(match.Result('${column}'));
					error.Line        = Int32.Parse(match.Result('${line}'));
					error.FileName    = Path.GetFullPath(match.Result('${file}'));
					//error.IsWarning   = match.Result('${error}') == "warning";
					error.ErrorNumber = match.Result('${number}');
					error.ErrorText   = match.Result('${message}');
				else:
					match = _generalError.Match(curLine); // try to match general csc errors
					if match.Success:
						error.IsWarning   = match.Result('${error}') == "warning";
						error.ErrorNumber = match.Result('${number}');
						error.ErrorText   = match.Result('${message}');
					else:
						// give up and skip the line
						curLine = sr.ReadLine()
						continue
				
				cr.Errors.Add(error);
				
				curLine = sr.ReadLine()
		return DefaultCompilerResult(cr, compilerOutput.ToString())

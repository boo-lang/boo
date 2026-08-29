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
import System.Collections
import System.IO
import System.Diagnostics
import System.Text
import System.Reflection
import System.CodeDom.Compiler

import Boo.Lang.Compiler.CompilerWarning as BooWarning
import Boo.Lang.Compiler.CompilerError as BooError

import ICSharpCode.Core.Services

import ICSharpCode.SharpDevelop.Internal.Project
import ICSharpCode.SharpDevelop.Gui
import ICSharpCode.SharpDevelop.Services

class BooBindingCompilerManager:
	_booLibNotFound = "Boo.Dll was not found in the boo addin directory"
	
	def GetCompiledOutputName(fileName as string) as string:
		return Path.ChangeExtension(fileName, ".exe")
	
	def GetCompiledOutputName(p as BooProject) as string:
		compilerparameters as BooCompilerParameters = p.ActiveConfiguration
		
		fileUtilityService as FileUtilityService = ServiceManager.Services.GetService(typeof(FileUtilityService))
		exe = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly
		if (compilerparameters.CompileTarget == CompileTarget.Library):
			return exe + ".dll"
		else:
			return exe + ".exe"
	
	def CanCompile(fileName as string) as bool:
		return Path.GetExtension(fileName).ToLower() == ".boo"
	
	private def MakeError(text as string):
		cr = CompilerResults(TempFileCollection())
		cr.Errors.Add(CompilerError(ErrorText: text))
		return DefaultCompilerResult(cr, text)
	
	private def Compile(compilerparameters as BooCompilerParameters, fileNames as (string), outputFile as string, p as IProject) as ICompilerResult:
		messageService as IMessageService = ServiceManager.Services.GetService(typeof(IMessageService))
		cr as CompilerResults = null
		
		booDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
		booLib = Path.Combine(booDir, "Boo.Lang.dll")
		if not File.Exists(booLib):
			messageService.ShowError(_booLibNotFound)
			return MakeError(_booLibNotFound)
			
		outputDirectory = Path.GetDirectoryName(outputFile)
		try:
			File.Copy(booLib, Path.Combine(outputDirectory, Path.GetFileName(booLib)), true)
		except:
			pass
		
		if compilerparameters.CompileTarget == CompileTarget.WinExe:
			WriteManifestFile(outputFile)
		
		compiler as BooCompilerWrapper = BooCompilerWrapper()
		compiler.SetOptions(compilerparameters.CurrentCompilerOptions)
		compiler.OutputFile = outputFile
		
		for fileName as string in fileNames:
			compiler.AddInputFile(Path.GetFullPath(fileName))
		
		if p != null:
			// write references
			for lib as ProjectReference in p.ProjectReferences:
				compiler.AddReference(lib.GetReferencedFileName(p))
			// write embedded resources
			for finfo as ProjectFile in p.ProjectFiles:
				if finfo.Subtype != Subtype.Directory and finfo.BuildAction == BuildAction.EmbedAsResource:
					compiler.AddResource(finfo.Name)
			p.CopyReferencesToOutputPath(true)
		
		result = compiler.Run()
		cr = CompilerResults(TempFileCollection())
		
		compilerOutput = StringBuilder()
		for line in StringReader(result):
			print line
			
			compilerOutput.Append(line)
			compilerOutput.Append(Environment.NewLine)
			
			error = CompilerError()
			
			match = /^(.+)\((\d+),(\d+)\):\s([\w\d]+):\s(.+)$/.Match(line)
			if match.Success:			
				groups = match.Groups
				filename = groups[1].Value
				lineNumber = int.Parse(groups[2].Value)
				column = int.Parse(groups[3].Value)
				code = groups[4].Value
				message = groups[5].Value
				
				error.ErrorNumber = code
				error.ErrorText = message
				error.IsWarning = code.StartsWith("BCW")				
				if lineNumber >= 0:
					error.Column   = column
					error.Line     = lineNumber
					error.FileName = filename
			else:
				match = /^([\w\d]+):\s(.+)$/.Match(line)
				if match.Success:
					error.ErrorNumber = match.Groups[1].Value
					error.ErrorText = match.Groups[2].Value
				else:
					match = /^(.+):\s(.+)$/.Match(line)
					if match.Success:
						error.ErrorText = line
					else:
						continue
					
			cr.Errors.Add(error)
		
		compiler = null		
		
		return DefaultCompilerResult(cr, compilerOutput.ToString())
	
	static def MyResolveEventHandler(sender, e as ResolveEventArgs) as Assembly:
		Console.WriteLine("Resolving ${e.Name}")
		return null
	
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

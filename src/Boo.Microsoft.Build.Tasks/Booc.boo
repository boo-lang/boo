#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Microsoft.Build.Tasks

import Microsoft.Build.Framework
import Microsoft.Build.Tasks
import Microsoft.Build.Utilities
import System
import System.Diagnostics
import System.IO
import System.Globalization
import System.Text.RegularExpressions
import System.Threading

class Booc(ManagedCompiler):
"""
Represents the Boo compiler MSBuild task.

Authors:
	Sorin Ionescu (sorin.ionescu@gmail.com)
"""
	def constructor():
		NoLogo = true
	
	Pipeline:
	"""
	Gets/sets a specific pipeline to add to the compiler process.
	"""
		get:
			return Bag['Pipelines'] as string
		set:
			Bag['Pipelines'] = value
	NoStandardLib:
	"""
	Gets/sets if we want to link to the standard libraries or not.
	"""
		get:
			return GetBoolParameterWithDefault("NoStandardLib", false)
		set:
			Bag['NoStandardLib'] = value
	WhiteSpaceAgnostic:
	"""
	Gets/sets if we want to use whitespace agnostic mode.
	"""
		get:
			return GetBoolParameterWithDefault("WhiteSpaceAgnostic", false)
		set:
			Bag['WhiteSpaceAgnostic'] = value
	Ducky:
	"""
	Gets/sets if we want to use ducky mode.
	"""
		get:
			return GetBoolParameterWithDefault("Ducky", false)
		set:
			Bag['Ducky'] = value
	Verbosity:
	"""
	Gets/sets the verbosity level.
	"""
		get:
			return Bag['Verbosity'] as string
		set:
			Bag['Verbosity'] = value
	
	Culture:
	"""
	Gets/sets the culture.
	"""
		get:
			return Bag['Culture'] as string
		set:
			Bag['Culture'] = value
	
	SourceDirectory:
	"""
	Gets/sets the source directory.
	"""
		get:
			return Bag['Source Directory'] as string
		set:
			Bag['Source Directory'] = value
	
	DefineSymbols:
	"""
	Gets/sets the conditional compilation symbols.
	"""
		get:
			return Bag['DefineSymbols'] as string
		set:
			Bag['DefineSymbols'] = value
	
	CheckForOverflowUnderflow:
	"""
	Gets/sets if integer overlow checking is enabled.
	"""
		get:
			return GetBoolParameterWithDefault("CheckForOverflowUnderflow", true)
		set:
			Bag['CheckForOverflowUnderflow'] = value
	
	DisabledWarnings:
	"""
	Gets/sets a comma-separated list of warnings that should be disabled.
	"""
		get:
			return Bag['DisabledWarnings'] as string
		set:
			Bag['DisabledWarnings'] = value
	
	WarningsAsErrors:
	"""
	Gets/sets a comma-separated list of warnings that should be treated as errors.
	"""
		get:
			return Bag['WarningsAsErrors'] as string
		set:
			Bag['WarningsAsErrors'] = value
	
	Strict:
	"""
	Gets/sets whether strict mode is enabled.
	"""
		get:
			return GetBoolParameterWithDefault("Strict", false)
		set:
			Bag['Strict'] = value
	AllowUnsafeBlocks:
	"""
	Allows to compile unsafe code.
	"""
		get:
			return GetBoolParameterWithDefault("AllowUnsafeBlocks", false)
		set:
			Bag['AllowUnsafeBlocks'] = value
	Platform:
	"""
	Specifies target platform (anycpu, x86, x64 or itanium)
	"""
		get:
			return Bag['Platform'] as string
		set:
			Bag['Platform'] = value
	
	ToolName:
	"""
	Gets the tool name.
	"""
		get:
			return "booc.exe"
			
	GenerateFullPaths:
	"""
	If set to true the task will output warnings and errors with full file paths
	"""
		get:
			return GetBoolParameterWithDefault("GenerateFullPaths", false)
		set:
			Bag["GenerateFullPaths"] = value
		
	
	override def Execute():
	"""
	Executes the task.
	
	Returns:
		true if the task completed successfully; otherwise, false.
	"""
		boocCommandLine = CommandLineBuilderExtension()
		AddResponseFileCommands(boocCommandLine)
		
		warningPattern = regex(
			'^(?<file>.*?)(\\((?<line>\\d+),(?<column>\\d+)\\):)?' +
				'(\\s?)(?<code>BCW\\d{4}):(\\s)WARNING:(\\s)(?<message>.*)$',
			RegexOptions.Compiled)
		# Captures the file, line, column, code, and message from a BOO warning
		# in the form of: Program.boo(1,1): BCW0000: WARNING: This is a warning.
		
		errorPattern = regex(
			'^(((?<file>.*?)\\((?<line>\\d+),(?<column>\\d+)\\): )?' +
				'(?<code>BCE\\d{4})|(?<errorType>Fatal) error):' +
				'( Boo.Lang.Compiler.CompilerError:)?' + 
				' (?<message>.*?)($| --->)',
			RegexOptions.Compiled |
				RegexOptions.ExplicitCapture |
				RegexOptions.Multiline)
		/* 
		 * Captures the file, line, column, code, error type, and message from a
		 * BOO error of the form of:
		 * 1. Program.boo(1,1): BCE0000: This is an error.
		 * 2. Program.boo(1,1): BCE0000: Boo.Lang.Compiler.CompilerError:
		 *    	This is an error. ---> Program.boo:4:19: This is an error
		 * 3. BCE0000: This is an error.
		 * 4. Fatal error: This is an error.
		 *
		 * The second line of the following error format is not cought because 
		 * .NET does not support if|then|else in regular expressions,
		 * and the regex will be horrible complicated.  
		 * The second line is as worthless as the first line.
		 * Therefore, it is not worth implementing it.
		 *
		 * 	Fatal error: This is an error.
		 * 	Parameter name: format.
		 */
		
		buildSuccess = true
		outputLine = String.Empty
		errorLine = String.Empty
		readingDoneEvents = (ManualResetEvent(false), ManualResetEvent(false))
		
		boocProcessStartInfo = ProcessStartInfo(
			FileName: GenerateFullPathToTool(),
			Arguments: boocCommandLine.ToString(),
			ErrorDialog: false,
			CreateNoWindow: true,
			RedirectStandardError: true,
			RedirectStandardInput: false,
			RedirectStandardOutput: true,
			UseShellExecute: false)
		
		boocProcess = Process(StartInfo: boocProcessStartInfo)
		
		parseOutput = def(line as string):
			warningPatternMatch = warningPattern.Match(line)
			errorPatternMatch = errorPattern.Match(line)
		
			if warningPatternMatch.Success:
				lineOut = 0
				columnOut = 0
				int.TryParse(warningPatternMatch.Groups['line'].Value, lineOut)
				int.TryParse(warningPatternMatch.Groups['column'].Value, columnOut)
				Log.LogWarning(
					null,
					warningPatternMatch.Groups['code'].Value,
					null,
					GetFilePathToWarningOrError(warningPatternMatch.Groups['file'].Value),
					lineOut,
					columnOut,
					0,
					0,
					warningPatternMatch.Groups['message'].Value)
		
			elif errorPatternMatch.Success:					
				code = errorPatternMatch.Groups['code'].Value
				code = 'BCE0000' if string.IsNullOrEmpty(code)
				file = GetFilePathToWarningOrError(errorPatternMatch.Groups['file'].Value)
				file = 'BOOC' if string.IsNullOrEmpty(file)
				
				try:
					lineNumber = int.Parse(
						errorPatternMatch.Groups['line'].Value,
						NumberStyles.Integer)
						
				except as FormatException:
					lineNumber = 0

				try:
					columnNumber = int.Parse(
						errorPatternMatch.Groups['column'].Value,
						NumberStyles.Integer)
						
				except as FormatException:
					columnNumber = 0

				Log.LogError(
					errorPatternMatch.Groups['errorType'].Value.ToLower(),
					code,
					null,
					file,
					lineNumber,
					columnNumber,
					0,
					0,
					errorPatternMatch.Groups['message'].Value)
		
				buildSuccess = false
		
			else:
				Log.LogMessage(MessageImportance.Normal, line)
				
		readStandardOutput = def():
			while true:
				outputLine = boocProcess.StandardOutput.ReadLine()
		
				if outputLine:
					parseOutput(outputLine)
					
				else:
					readingDoneEvents[0].Set()
					break

		readStandardError = def():
			while true:
				errorLine = boocProcess.StandardError.ReadLine()

				if errorLine:
					parseOutput(errorLine)
					
				else:
					readingDoneEvents[1].Set()
					break
		
		standardOutputReadingThread = Thread(readStandardOutput as ThreadStart)	
		standardErrorReadingThread = Thread(readStandardError as ThreadStart)
		# Two threads are required (MSDN); otherwise, a deadlock WILL occur.
		
		try:
			boocProcess.Start()
			
			Log.LogMessage(
				MessageImportance.High,
				"${ToolName} ${boocProcess.StartInfo.Arguments}",
				null)
				
			standardOutputReadingThread.Start()
			standardErrorReadingThread.Start()
			
			WaitHandle.WaitAny((readingDoneEvents[0],))
			WaitHandle.WaitAny((readingDoneEvents[1],))
			# MSBuild runs on an STA thread, and WaitHandle.WaitAll()
			# is not supported.
			
			boocProcess.WaitForExit()
			if boocProcess.ExitCode != 0:
				if buildSuccess:
					// Report an error if booc exits with error code but we didn't
					// receive any error.
					Log.LogError("booc exited with code ${boocProcess.ExitCode}")
				buildSuccess = false
		except e as Exception:
			Log.LogErrorFromException(e)
			buildSuccess = false
			
		ensure:
			boocProcess.Close()

		return buildSuccess
	
	protected override def AddCommandLineCommands(
		commandLine as CommandLineBuilderExtension):
	"""
	Adds command line commands.
	
	Remarks:
		It prevents <ManagedCompiler> from adding the standard commands.
	"""
			pass
	
	protected override def AddResponseFileCommands(
		commandLine as CommandLineBuilderExtension):
	"""
	Generates the Boo compiler command line.
	
	Returns:
		The Boo compiler command line.
	"""	
		commandLine.AppendSwitchIfNotNull('-t:', TargetType)
		commandLine.AppendSwitchIfNotNull('-o:', OutputAssembly)
		commandLine.AppendSwitchIfNotNull('-c:', Culture)
		commandLine.AppendSwitchIfNotNull('-srcdir:', SourceDirectory)
		commandLine.AppendSwitchIfNotNull('-keyfile:', KeyFile)
		commandLine.AppendSwitchIfNotNull('-keycontainer:', KeyContainer)
		commandLine.AppendSwitchIfNotNull('-p:', Pipeline)
		commandLine.AppendSwitchIfNotNull('-define:', DefineSymbols)
		commandLine.AppendSwitchIfNotNull("-lib:", AdditionalLibPaths, ",")
		commandLine.AppendSwitchIfNotNull('-nowarn:', DisabledWarnings)
		commandLine.AppendSwitchIfNotNull('-platform:', Platform)
		
		if TreatWarningsAsErrors:
			commandLine.AppendSwitch('-warnaserror') // all warnings are errors
		else:
			commandLine.AppendSwitchIfNotNull('-warnaserror:', WarningsAsErrors) // only specific warnings are errors
		
		if NoLogo:
			commandLine.AppendSwitch('-nologo')
		if NoConfig:
			commandLine.AppendSwitch('-noconfig')
		if NoStandardLib:
			commandLine.AppendSwitch('-nostdlib')
		if DelaySign:
			commandLine.AppendSwitch('-delaysign')
		if WhiteSpaceAgnostic:
			commandLine.AppendSwitch('-wsa')
		if Ducky:
			commandLine.AppendSwitch('-ducky')
		if Utf8Output:
			commandLine.AppendSwitch('-utf8')
		if Strict:
			commandLine.AppendSwitch('-strict')
		if AllowUnsafeBlocks:
			commandLine.AppendSwitch('-unsafe')
		
		if EmitDebugInformation:
			commandLine.AppendSwitch('-debug+')
		else:
			commandLine.AppendSwitch('-debug-')
		
		if CheckForOverflowUnderflow:
			commandLine.AppendSwitch('-checked+')
		else:
			commandLine.AppendSwitch('-checked-')
		
		if ResponseFiles:
			for rsp in ResponseFiles:
				commandLine.AppendSwitchIfNotNull("@", rsp.ItemSpec)				

		if References:
			for reference in References:
				commandLine.AppendSwitchIfNotNull('-r:', reference.ItemSpec)
				
		if Resources:
			for resource in Resources:
				commandLine.AppendSwitchIfNotNull('-resource:', resource.ItemSpec)
		
		if Verbosity:
			if string.Compare(
					Verbosity,
					'Normal',
					StringComparison.InvariantCultureIgnoreCase) == 0:
				pass
				
			elif string.Compare(
					Verbosity,
					'Warning',
					StringComparison.InvariantCultureIgnoreCase) == 0:
					
				commandLine.AppendSwitch('-v')
				
			elif string.Compare(
					Verbosity,
					'Info',
					StringComparison.InvariantCultureIgnoreCase) == 0:
					
				commandLine.AppendSwitch('-vv')
				
			elif string.Compare(
					Verbosity,
					'Verbose',
					StringComparison.InvariantCultureIgnoreCase) == 0:
					
				commandLine.AppendSwitch('-vvv')
			
			else:
				Log.LogErrorWithCodeFromResources(
					'Vbc.EnumParameterHasInvalidValue',
					'Verbosity',
					Verbosity,
					'Normal, Warning, Info, Verbose')
					
		commandLine.AppendFileNamesIfNotNull(Sources, ' ')
		
	protected override def GenerateFullPathToTool():
	"""
	Generats the full path to booc.exe.
	"""
		path = ""
		
		if ToolPath:
			path = Path.Combine(ToolPath, ToolName)
		
		return path if File.Exists(path)
		
		path = Path.Combine(
			Path.GetDirectoryName(typeof(Booc).Assembly.Location),
			ToolName)
		
		return path if File.Exists(path)
		
		path = ToolLocationHelper.GetPathToDotNetFrameworkFile(
			ToolName,
			TargetDotNetFrameworkVersion.VersionLatest)
		
		return path if File.Exists(path)
		
		/* //removed this error message for mono compatibility
		Log.LogErrorWithCodeFromResources(
			"General.FrameworksFileNotFound",
			ToolName,
			ToolLocationHelper.GetDotNetFrameworkVersionFolderPrefix(
				TargetDotNetFrameworkVersion.Version20))
		*/
		path = "booc"
						
		return path
	
	private def GetFilePathToWarningOrError(file as string):
		if GenerateFullPaths:
			return Path.GetFullPath(file)
		else:
			return file

#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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



namespace booc

import System
import System.Collections.Generic
import System.Diagnostics
import System.Globalization
import System.IO
import System.Linq
import System.Linq.Enumerable
import System.Reflection
import System.Threading
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Resources
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Compiler.Util
import Boo.Lang.Environments
import Boo.Lang.Resources

public class CommandLineParser:

	public static def ParseInto(options as CompilerParameters, *commandLine as (string)):
		CommandLineParser(commandLine, options)

	
	private final _options as CompilerParameters

	private final _processedResponseFiles as Set[of string] = Set[of string]()

	private final _references as List[of string] = List[of string]()

	private final _packages as List[of string] = List[of string]()

	private _noConfig as bool

	private _pipelineName as string

	private _debugSteps as bool

	
	private def constructor(args as IEnumerable[of string], options as CompilerParameters):
		_options = options
		_options.GenerateInMemory = false
		
		tempLibPaths = _options.LibPaths.ToArray()
		_options.LibPaths.Clear()
		
		Parse(args)
		
		//move standard libpaths below any new ones:
		_options.LibPaths.Extend(tempLibPaths)
		
		if _options.StdLib:
			_options.LoadDefaultReferences()
		elif not _noConfig:
			_references.Insert(0, 'mscorlib')
		
		LoadReferences()
		ConfigurePipeline()
		
		if _options.TraceInfo:
			_options.Pipeline.BeforeStep += OnBeforeStep
			_options.Pipeline.AfterStep += OnAfterStep
	
	private def Parse(commandLine as IEnumerable[of string]):
		warnings as string
		noLogo = false
		args = ExpandResponseFiles(commandLine.Select({ s | return StripQuotes(s) }))
		AddDefaultResponseFile(args)
		for arg in args:
			if '-' == arg:
				_options.Input.Add(StringInput('<stdin>', Consume(Console.In)))
				continue 
			if not IsFlag(arg):
				_options.Input.Add(FileInput(StripQuotes(arg)))
				continue 
			if '-utf8' == arg:
				continue 
			converterGeneratedName1 = arg[1]
			if converterGeneratedName1 == char('h'):
				if (arg == '-help') or (arg == '-h'):
					Help()
			elif converterGeneratedName1 == char('w'):
				if arg == '-wsa':
					_options.WhiteSpaceAgnostic = true
				elif arg == '-warnaserror':
					_options.WarnAsError = true
				elif arg.StartsWith('-warnaserror:'):
					warnings = ValueOf(arg)
					for warning as string in warnings.Split(char(',')):
						_options.EnableWarningAsError(warning)
				elif arg.StartsWith('-warn:'):
					warnings__2 = ValueOf(arg)
					for warning as string in warnings__2.Split(char(',')):
						_options.EnableWarning(warning)
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('v'):
				_options.TraceLevel = TraceLevel.Warning
				if arg.Length > 2:
					converterGeneratedName2 = arg.Substring(1)
					if converterGeneratedName2 == 'vv':
						_options.TraceLevel = TraceLevel.Info
						MonitorAppDomain()
					elif converterGeneratedName2 == 'vvv':
						_options.TraceLevel = TraceLevel.Verbose
				else:
					_options.TraceLevel = TraceLevel.Warning
			elif converterGeneratedName1 == char('r'):
				if (arg.IndexOf(':') > 2) and (arg.Substring(1, 9) != 'reference'):
					converterGeneratedName3 = arg.Substring(1, 8)
					if converterGeneratedName3 == 'resource':
						AddResource(ValueOf(arg))
					else:
						InvalidOption(arg)
				else:
					assemblies = ValueOf(arg)
					for assemblyName in assemblies.Split(char(',')):
						_references.Add(assemblyName)
			elif converterGeneratedName1 == char('l'):
				converterGeneratedName4 = arg.Substring(1, 3)
				if converterGeneratedName4 == 'lib':
					ParseLib(arg)
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('n'):
				if arg == '-nologo':
					noLogo = true
				elif arg == '-noconfig':
					_noConfig = true
				elif arg == '-nostdlib':
					_options.StdLib = false
				elif arg == '-nowarn':
					_options.NoWarn = true
				elif arg.StartsWith('-nowarn:'):
					warnings = ValueOf(arg)
					for warning as string in warnings.Split(char(',')):
						_options.DisableWarning(warning)
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('o'):
				_options.OutputAssembly = ValueOf(arg)
			elif converterGeneratedName1 == char('t'):
				targetType as string = ValueOf(arg)
				converterGeneratedName5 = targetType
				if converterGeneratedName5 == 'library':
					_options.OutputType = CompilerOutputType.Library
				elif converterGeneratedName5 == 'exe':
					_options.OutputType = CompilerOutputType.ConsoleApplication
				elif converterGeneratedName5 == 'winexe':
					_options.OutputType = CompilerOutputType.WindowsApplication
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('p'):
				if arg.StartsWith('-pkg:'):
					packages as string = ValueOf(arg)
					_packages.Add(packages)
				elif arg.StartsWith('-platform:'):
					arch as string = ValueOf(arg).ToLowerInvariant()
					converterGeneratedName6 = arch
					if converterGeneratedName6 == 'anycpu':
						pass
					elif converterGeneratedName6 == 'x86':
						_options.Platform = 'x86'
					elif converterGeneratedName6 == 'x64':
						_options.Platform = 'x64'
					elif converterGeneratedName6 == 'itanium':
						_options.Platform = 'itanium'
					else:
						InvalidOption(arg, 'Valid platform types are: `anycpu\', `x86\', `x64\' or `itanium\'.')
				elif arg.StartsWith('-p:'):
					_pipelineName = StripQuotes(arg.Substring(3))
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('c'):
				converterGeneratedName7 = arg.Substring(1)
				if (converterGeneratedName7 == 'checked') or (converterGeneratedName7 == 'checked+'):
					_options.Checked = true
				elif converterGeneratedName7 == 'checked-':
					_options.Checked = false
				else:
					culture as string = arg.Substring(3)
					Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture)
			elif converterGeneratedName1 == char('s'):
				converterGeneratedName8 = arg.Substring(1, 6)
				if converterGeneratedName8 == 'srcdir':
					path as string = StripQuotes(arg.Substring(8))
					AddFilesForPath(path, _options)
				elif converterGeneratedName8 == 'strict-':
					pass
				elif (converterGeneratedName8 == 'strict') or (converterGeneratedName8 == 'strict+'):
					_options.Strict = true
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('k'):
				if arg.Substring(1, 7) == 'keyfile':
					_options.KeyFile = StripQuotes(arg.Substring(9))
				elif arg.Substring(1, 12) == 'keycontainer':
					_options.KeyContainer = StripQuotes(arg.Substring(14))
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('d'):
				converterGeneratedName9 = arg.Substring(1)
				if (converterGeneratedName9 == 'debug') or (converterGeneratedName9 == 'debug+'):
					_options.Debug = true
				elif converterGeneratedName9 == 'debug-':
					_options.Debug = false
				elif converterGeneratedName9 == 'ducky':
					_options.Ducky = true
				elif converterGeneratedName9 == 'debug-steps':
					_debugSteps = true
				elif converterGeneratedName9 == 'delaysign':
					_options.DelaySign = true
				elif arg.StartsWith('-d:') or arg.StartsWith('-define:'):
					symbols as (string) = ValueOf(arg).Split(*','.ToCharArray())
					for symbol as string in symbols:
						s_v as (string) = symbol.Split('='.ToCharArray(), 2)
						if s_v[0].Length < 1:
							continue 
						if _options.Defines.ContainsKey(s_v[0]):
							_options.Defines[s_v[0]] = (s_v[1] if (s_v.Length > 1) else null)
							//sv1 = 
							//ti as string = 
							TraceInfo("REPLACED DEFINE '$(s_v[0])' WITH VALUE '$((s_v[1] if (s_v.Length > 1) else string.Empty))'")
						else:
							_options.Defines.Add(s_v[0], (s_v[1] if (s_v.Length > 1) else null))
							TraceInfo("ADDED DEFINE '$(s_v[0])' WITH VALUE '$((s_v[1] if (s_v.Length > 1) else string.Empty))'")
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('e'):			
				converterGeneratedName10 = arg.Substring(1, 8)
				if converterGeneratedName10 == 'embedres':
					EmbedResource(ValueOf(arg))
				else:					
					InvalidOption(arg)
			elif converterGeneratedName1 == char('u'):			
				if arg == '-unsafe':
					_options.Unsafe = true
				else:
					InvalidOption(arg)
			elif converterGeneratedName1 == char('x'):			
				if arg.Substring(1).StartsWith('x-type-inference-rule-attribute'):
					attribute = ValueOf(arg)
					_options.Environment = DeferredEnvironment() {TypeInferenceRuleProvider: {return CustomTypeInferenceRuleProvider(attribute)}}
				else:
					InvalidOption(arg)
			elif arg == '--help':			
				Help()
			else:
				InvalidOption(arg)		
		if not noLogo:
			DoLogo()
	
	private static def ValueOf(arg as string) as string:
		return StripQuotes(arg.Substring(arg.IndexOf(':') + 1))
	
	private def ParseLib(arg as string):
		paths = TrimAdditionalQuote(ValueOf(arg))
		// TrimAdditionalQuote to work around nant bug with spaces on lib path
		if string.IsNullOrEmpty(paths):
			Console.Error.WriteLine(string.Format(Boo.Lang.Resources.StringResources.BooC_BadLibPath, arg))
			return
		for dir in paths.Split(char(',')):
			if Directory.Exists(dir):
				_options.LibPaths.Add(dir)
			else:
				Console.Error.WriteLine(string.Format(Boo.Lang.Resources.StringResources.BooC_BadLibPath, dir))

	private static def DoLogo():
		Console.WriteLine('Boo Compiler version {0} ({1})', Boo.Lang.Builtins.BooVersion, Boo.Lang.Runtime.RuntimeServices.RuntimeDisplayName)

	
	private static def Help():   
		Console.WriteLine("""Usage: booc [options] file1 ...
Options:
 -c:CULTURE           Sets the UI culture to be CULTURE
 -checked[+|-]        Turns on or off checked operations (default: +)
 -debug[+|-]          Generate debugging information (default: +)
 -define:S1[,Sn]      Defines symbols S1..Sn with optional values (=val) (-d:)
 -delaysign           Delays assembly signing
 -ducky               Turns on duck typing by default
 -embedres:FILE[,ID]  Embeds FILE with the optional ID
 -keycontainer:NAME   The key pair container used to strongname the assembly
 -keyfile:FILE        The strongname key file used to strongname the assembly
 -lib:DIRS            Adds the comma-separated DIRS to the assembly search path
 -noconfig            Does not load the standard configuration
 -nologo              Does not display the compiler logo
 -nostdlib            Does not reference any of the default libraries
 -nowarn[:W1,Wn]      Suppress all or a list of compiler warnings
 -o:FILE              Sets the output file name to FILE
 -p:PIPELINE          Sets the pipeline to PIPELINE
 -pkg:P1[,Pn]         References packages P1..Pn (on supported platforms)
 -platform:ARCH       Specifies target platform (anycpu, x86, x64 or itanium)
 -reference:A1[,An]   References assemblies (-r:)
 -resource:FILE[,ID]  Embeds FILE as a resource
 -srcdir:DIR          Adds DIR as a directory where sources can be found
 -strict              Turns on strict mode.
 -target:TYPE         Sets the target type (exe, library or winexe) (-t:)
 -unsafe              Allows to compile unsafe code.
 -utf8                Source file(s) are in utf8 format
 -v, -vv, -vvv        Sets verbosity level from warnings to very detailed
 -warn:W1[,Wn]        Enables a list of optional warnings.
 -warnaserror[:W1,Wn] Treats all or a list of warnings as errors
 -wsa                 Enables white-space-agnostic build\n""")
	
	private def EmbedResource(resourceFile as string):
		comma as int = resourceFile.LastIndexOf(char(','))
		if comma >= 0:
			resourceName as string = resourceFile.Substring(comma + 1)
			resourceFile = resourceFile.Substring(0, comma)
			_options.Resources.Add(NamedEmbeddedFileResource(resourceFile, resourceName))
		else:
			_options.Resources.Add(EmbeddedFileResource(resourceFile))

	
	private def AddResource(resourceFile as string):
		comma as int = resourceFile.LastIndexOf(char(','))
		if comma >= 0:
			resourceName as string = resourceFile.Substring(comma + 1)
			resourceFile = resourceFile.Substring(0, comma)
			_options.Resources.Add(NamedFileResource(resourceFile, resourceName))
		else:
			_options.Resources.Add(FileResource(resourceFile))

	
	private def ConfigurePipeline():
		pipeline = (CompilerPipeline.GetPipeline(_pipelineName) if (_pipelineName is not null) else CompileToFile())
		_options.Pipeline = pipeline
		if _debugSteps:
			stepDebugger = StepDebugger()
			pipeline.BeforeStep += stepDebugger.BeforeStep
			pipeline.AfterStep += stepDebugger.AfterStep

	private static def StripQuotes(s as string) as string:
		if (s.Length > 1) and (IsDelimitedBy(s, '"') or IsDelimitedBy(s, '\'')):
			return s.Substring(1, (s.Length - 2))
		return s

	private static def IsDelimitedBy(s as string, delimiter as string) as bool:
		return (s.StartsWith(delimiter) and s.EndsWith(delimiter))

	private static def TrimAdditionalQuote(s as string) as string:
		return (s.Substring(0, (s.Length - 1)) if s.EndsWith('"') else s)

	private class StepDebugger:

		private _last as string

		private _stopWatch as Stopwatch

		public def BeforeStep(sender as object, args as CompilerStepEventArgs):
			_stopWatch = Stopwatch.StartNew()

		public def AfterStep(sender as object, args as CompilerStepEventArgs):
			_stopWatch.Stop()
			Console.WriteLine('********* {0} - {1} *********', args.Step, _stopWatch.Elapsed)
			
			writer = StringWriter()
			args.Context.CompileUnit.Accept(BooPrinterVisitor(writer, BooPrinterVisitor.PrintOptions.PrintLocals))
			code = writer.ToString()
			if code != _last:
				Console.WriteLine(code)
			else:
				Console.WriteLine('no changes')
			_last = code

	private def LoadResponseFile(file as string) as List[of string]:
		file = Path.GetFullPath(file)
		if _processedResponseFiles.Contains(file):
			raise ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BCE0500, file))
		_processedResponseFiles.Add(file)
		if not File.Exists(file):
			raise ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BCE0501, file))
		
		arglist = List[of string]()
		try:
			using sr = StreamReader(file):
				line as string
				while (line = sr.ReadLine()) is not null:
					line = line.Trim()
					if (line.Length > 0) and (line[0] != char('#')):
						if line.StartsWith('@') and (line.Length > 2):
							arglist.AddRange(LoadResponseFile(line.Substring(1)))
						else:
							arglist.Add(StripQuotes(line))
		except converterGeneratedName11 as ApplicationException:
			raise 
		except x as Exception:
			raise ApplicationException(string.Format(Boo.Lang.Resources.StringResources.BCE0502, file), x)
		return arglist

	private def ExpandResponseFiles(args as IEnumerable[of string]) as List[of string]:
		result = List[of string]()
		for arg in args:
			if arg.StartsWith('@') and (arg.Length > 2):
				result.AddRange(LoadResponseFile(arg.Substring(1)))
			else:
				result.Add(arg)
		return result

	private def AddDefaultResponseFile(args as List[of string]):
		if not args.Contains('-noconfig'):
			file as string = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 'booc.rsp')
			if File.Exists(file):
				args.InsertRange(0, LoadResponseFile(file))

	private stepStopwatch as Stopwatch

	private def OnBeforeStep(sender as object, args as CompilerStepEventArgs):
		args.Context.TraceEnter('Entering {0}', args.Step)
		stepStopwatch = Stopwatch.StartNew()

	private def OnAfterStep(sender as object, args as CompilerStepEventArgs):
		stepStopwatch.Stop()
		args.Context.TraceLeave('Leaving {0} ({1}ms)', args.Step, stepStopwatch.ElapsedMilliseconds)

	private def InvalidOption(arg as string):
		InvalidOption(arg, null)

	private def InvalidOption(arg as string, message as string):
		Console.Error.WriteLine(StringResources.BooC_InvalidOption, arg, message)

	private static def IsFlag(arg as string) as bool:
		return (arg[0] == char('-'))

	private static def AddFilesForPath(path as string, options as CompilerParameters):
		for fname in Directory.GetFiles(path, '*.boo'):
			if not fname.EndsWith('.boo'):
				continue 
			options.Input.Add(FileInput(Path.GetFullPath(fname)))
		
		for dirName in Directory.GetDirectories(path):
			AddFilesForPath(dirName, options)
	
	private def OnAssemblyLoad(sender as object, args as AssemblyLoadEventArgs):
		TraceInfo('ASSEMBLY LOADED: ' + GetAssemblyLocation(args.LoadedAssembly))

	private static def GetAssemblyLocation(a as Assembly) as string:
		loc as string
		try:
			loc = a.Location
		except converterGeneratedName12 as Exception:
			loc = ('<dynamic>' + a.FullName)
		return loc

	private def MonitorAppDomain():
		if _options.TraceInfo:
			AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad
			for a in AppDomain.CurrentDomain.GetAssemblies():
				TraceInfo('ASSEMBLY AT STARTUP: ' + GetAssemblyLocation(a))

	private def TraceInfo(s as string):
		if _options.TraceInfo:
			Console.Error.WriteLine(s)

	private static def Consume(reader as TextReader) as string:
		writer = StringWriter()
		line = reader.ReadLine()
		while line is not null:
			writer.WriteLine(line)
			line = reader.ReadLine()
		return writer.ToString()

	private def LoadReferences():
		for r in _references:
			_options.References.Add(_options.LoadAssembly(r, true))
		for p in _packages:
			_options.LoadReferencesFromPackage(p)

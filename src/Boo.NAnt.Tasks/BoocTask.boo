#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.NAnt

import System
import System.IO
import System.Text
import System.Text.RegularExpressions
import NAnt.Core.Attributes
import NAnt.Core.Types
import NAnt.DotNet.Types

import NAnt.DotNet.Tasks
import System.Reflection

[TaskName('booc')]
public class BoocTask(CompilerBase):
	#region Private Instance Fields
	private _debugOutput as DebugOutput = DebugOutput.Enable
	private _exe as string
	
	private _useruntime = true //keep true for mono compatibility (don't call booc.exe directly)
	private _noconfig = false
	private _nostdlib = false
	private _wsa = false
	private _ducky = false
	private _checked = true
	private _defineSymbols as string = null
	private _embed = FileSet()
	private _pipeline as string
	private _strict = false
	private _unsafe = false
	private _platform as string

	#endregion Private Instance Fields
	#region Private Static Fields
	private static _classNameRegex = Regex('^((?<comment>/\\*.*?(\\*/|$))|[\\s\\.\\{]+|class\\s+(?<class>\\w+)|(?<keyword>\\w+))*')

	private static _namespaceRegex = Regex('^((?<comment>/\\*.*?(\\*/|$))|[\\s\\.\\{]+|namespace\\s+(?<namespace>(\\w+(\\.\\w+)*)+)|(?<keyword>\\w+))*')

	#endregion Private Static Fields
	
	def constructor():
		SupportsKeyFile = true
		SupportsKeyContainer = true
		SupportsPackageReferences = true
	
	[FrameworkConfigurable("exename")]
	[TaskAttribute('exename')]
	public override ExeName as string:
		get:
			return _exe
		set:
			_exe = value
	
	[FrameworkConfigurable("useruntimeengine")]
	[TaskAttribute('useruntimeengine')]
	override UseRuntimeEngine as bool:
		get:
			return _useruntime
		set:
			_useruntime = value
	
	#region Public Instance Properties

	[TaskAttribute('debug')]
	public DebugOutput as DebugOutput:
		get:
			return _debugOutput
		set:
			_debugOutput = value

	[TaskAttribute('pipeline')]
	public Pipeline:
		get:
			return _pipeline
		set:
			_pipeline = value

	public Debug as bool:
		get:
			return (DebugOutput != DebugOutput.None)
		set:
			if value:
				DebugOutput = DebugOutput.Enable
			else:
				DebugOutput = DebugOutput.None

	[FrameworkConfigurable('noconfig')]
	[TaskAttribute('noconfig')]
	[BooleanValidator]
	public NoConfig as bool:
		get:
			return _noconfig
		set:
			_noconfig = value
			
	[FrameworkConfigurable('nostdlib')]
	[TaskAttribute('nostdlib')]
	[BooleanValidator]
	public NoStdLib as bool:
		get:
			return _nostdlib
		set:
			_nostdlib = value
	
	[TaskAttribute('wsa')]
	[BooleanValidator]
	public WhiteSpaceAgnostic as bool:
		get:
			return _wsa
		set:
			_wsa = value
	
	[TaskAttribute('ducky')]
	[BooleanValidator]
	public Ducky as bool:
		get:
			return _ducky
		set:
			_ducky = value

	[TaskAttribute('checked')]
	[BooleanValidator]
	public Checked as bool:
		get:
			return _checked
		set:
			_checked = value

	[TaskAttribute('define')]
	public DefineSymbols as string:
		get:
			return _defineSymbols
		set:
			_defineSymbols = value

	[BuildElement("embed")]
	Embed:
		get:
			return _embed
		set:
			_embed = value

	[TaskAttribute('strict')]
	[BooleanValidator]
	public Strict as bool:
		get:
			return _strict
		set:
			_strict = value

	[TaskAttribute('unsafe')]
	[BooleanValidator]
	public Unsafe as bool:
		get:
			return _unsafe
		set:
			_unsafe = value

	[TaskAttribute('platform')]
	public Platform as string:
		get:
			return _platform
		set:
			_platform = value


	override public SupportsNoWarnList as bool:
		get:
			return true
		set:
			pass

	override public SupportsWarnAsErrorList as bool:
		get:
			return true
		set:
			pass


	private def FindBooc() as string:
		path as string
		dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
		if Project.TargetFramework:
			frameworkname = Project.TargetFramework.Name
			path = Path.Combine(Path.Combine(dir, frameworkname), "booc.exe")
			if File.Exists(path):
				return path
			//treat mono-1.0==net-1.1 and mono-##==net-##
			frameworkname = frameworkname.Replace("mono-1.0","net-1.1").Replace("mono-","net-")
			path = Path.Combine(Path.Combine(dir, frameworkname), "booc.exe")
			if File.Exists(path):
				return path
		path = Path.Combine(dir, "booc.exe")
		if File.Exists(path):
			return path
		_useruntime = false
		return "booc" //try booc in PATH


	protected override def ExecuteTask():
		if not ExeName or ExeName == string.Empty:
			ExeName = FindBooc()
		super()


	#endregion Public Instance Properties
	#region Override implementation of CompilerBase
	protected override def WriteOptions(writer as TextWriter):
		if DebugOutput != DebugOutput.None:
			WriteOption(writer, 'debug')
		else:
			WriteOption(writer, 'debug-')
		if NoConfig and (not Arguments.Contains('-noconfig')):
			Arguments.Add(Argument('-noconfig'))
		if NoStdLib:
			WriteOption(writer, "nostdlib")
		if Verbose:
			WriteOption(writer, "vv")
		if WhiteSpaceAgnostic:
			WriteOption(writer, "wsa")
		if Ducky:
			WriteOption(writer, "ducky")
		if not Checked:
			WriteOption(writer, "checked-")
		if Strict:
			WriteOption(writer, "strict")
		if Unsafe:
			WriteOption(writer, "unsafe")
		if Platform:
			WriteOption(writer, "platform", _platform)
		if DefineSymbols is not null:
			WriteOption(writer, "define", DefineSymbols)
		if Pipeline:
			WriteOption(writer, "p", _pipeline)
		for embed in _embed.FileNames:
			WriteOption(writer, "embedres", embed)


	protected override def WriteOption(writer as TextWriter, name as string):
		writer.WriteLine("-{0}", name)
		
	protected override def WriteOption(writer as TextWriter, name as string, value as string):
		if " " in value and not IsQuoted(value):
			writer.WriteLine("-{0}:\"{1}\"", name, value)
		else:
			writer.WriteLine("-{0}:{1}", name, value)

	protected override def WriteNoWarnList(writer as TextWriter):
		if SuppressWarnings.Count > 0:
			sb = StringBuilder()
			for warning as CompilerWarning in SuppressWarnings:
				if warning.IfDefined and not warning.UnlessDefined:
					sb.Append(warning.Number)
					WriteOption(writer, "nowarn", sb.ToString())

	protected override def WriteWarningsAsError(writer as TextWriter):
		if WarnAsError:
			WriteOption(writer, "warnaserror")
			return
		if WarningAsError.Includes.Count > 0:
			sb = StringBuilder()
			for warning as CompilerWarning in WarningAsError.Includes:
				if warning.IfDefined and not warning.UnlessDefined:
					sb.Append(warning.Number)
			WriteOption(writer, "warnaserror", sb.ToString())


	def IsQuoted(value as string):
		return value.StartsWith("\"") and value.EndsWith("\"")
	
	public Extension as string:
		get:
			return 'boo'

	protected ClassNameRegex as Regex:
		get:
			return _classNameRegex

	protected NamespaceRegex as Regex:
		get:
			return _namespaceRegex
	#endregion Override implementation of CompilerBase


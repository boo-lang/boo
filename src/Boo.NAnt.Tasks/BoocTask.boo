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
import System.Globalization
import System.IO
import System.Text.RegularExpressions
import NAnt.Core
import NAnt.Core.Attributes
import NAnt.Core.Types
import NAnt.Core.Util
import NAnt.DotNet.Types

import NAnt.DotNet.Tasks
import System.Reflection

[TaskName('booc')]
public class BoocTask(CompilerBase):
	#region Private Instance Fields
	private _debugOutput as DebugOutput = DebugOutput.None
	private _exe as string
	
	private _useruntime = true //keep true for mono compatibility (don't call booc.exe directly)
	private _noconfig = false
	private _nostdlib = false
	
	#endregion Private Instance Fields
	#region Private Static Fields
	private static _classNameRegex = Regex('^((?<comment>/\\*.*?(\\*/|$))|[\\s\\.\\{]+|class\\s+(?<class>\\w+)|(?<keyword>\\w+))*')

	private static _namespaceRegex = Regex('^((?<comment>/\\*.*?(\\*/|$))|[\\s\\.\\{]+|namespace\\s+(?<namespace>(\\w+(\\.\\w+)*)+)|(?<keyword>\\w+))*')

	#endregion Private Static Fields
	
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

	public Debug as bool:
		get:
			return (DebugOutput != DebugOutput.None)
		set:
			DebugOutput = DebugOutput.Enable

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
	
	private def FindBooc() as string:
		path as string
		dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
		if Project.TargetFramework:
			path = Path.Combine(dir, Project.TargetFramework.Name)
			path = Path.Combine(path, "booc.exe")
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
		converterGeneratedName1 = DebugOutput
		// handle debug builds.
		if converterGeneratedName1 == DebugOutput.None:
			pass
		else:
			if converterGeneratedName1 == DebugOutput.Enable:
				WriteOption(writer, 'debug')
			else:
				if converterGeneratedName1 == DebugOutput.Full:
					WriteOption(writer, 'debug')
				else:
					raise BuildException(string.Format(CultureInfo.InvariantCulture, ResourceUtils.GetString('NA2011'), DebugOutput), Location)
		if NoConfig and (not Arguments.Contains('-noconfig')):
			Arguments.Add(Argument('-noconfig'))
		if NoStdLib:
			WriteOption(writer, "nostdlib")
	
	protected override def WriteOption(writer as TextWriter, name as string):
		writer.WriteLine("-{0}", name)
		
	protected override def WriteOption(writer as TextWriter, name as string, value as string):
		if value.IndexOf(" ") > 0 and (not value.StartsWith("\"")
						or not value.EndsWith("\"")):
			writer.WriteLine("-{0}:\"{1}\"", name, value)
		else:
			writer.WriteLine("-{0}:{1}", name, value)
	
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


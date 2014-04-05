#region license
// Copyright (c) 2013 Harald Meyer auf'm Hofe (harald_meyer@users.sourceforge.net)
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

namespace Boo.Lang.Interpreter

import System
import System.Text
import System.Collections
import System.IO
import Boo.Lang.Interpreter.ColorScheme

class CmdDescr:
	public property Descr as CmdDeclarationAttribute
	public property Module as CmdClassAttribute
	public property Method as MethodInfo
	
	def constructor(descr, module, method):
		self.Descr = descr
		self.Module = module
		self.Method = method
	override def ToString():
		return "${self.Descr.Name}, shell command module ${self.Module.Name}"

class CmdExecution:
"""Collects and executes shell commands."""
	
	_interpreter as InteractiveInterpreter
	
	public def constructor(interpreter as InteractiveInterpreter):
		_interpreter = interpreter
		AppDomain.CurrentDomain.AssemblyLoad += def(sender, evt as AssemblyLoadEventArgs):
			self.CollectCmds(evt.LoadedAssembly)
		self.AddCmdObject(self)
		self.AddCmdObject(ColorScheme)
	
	_cmdObjects = Generic.SortedList[of long, object]()
	"""Objects that are required for non-static ShellCmd.""" 
	
	public def AddCmdObject(cmdObject):
	"""The argument will be registered as an instance to be used for the
	   execution of shell commands."""
		self._cmdObjects.Add(cmdObject.GetType().TypeHandle.Value.ToInt64(), cmdObject)
		
	_collectedCmds as Generic.SortedList[of string, CmdDescr]
	_collectedCmdsHelp as Generic.List[of CmdDescr]
	def CollectCmds():
		if self._collectedCmds == null or self._collectedCmdsHelp == null:
			self._collectedCmds = Generic.SortedList[of string, CmdDescr]()
			self._collectedCmdsHelp = Generic.List[of CmdDescr]()
			for a in AppDomain.CurrentDomain.GetAssemblies():
				self.CollectCmds(a)
		return self._collectedCmdsHelp
	
	def CollectCmds(a as System.Reflection.Assembly):
		for t in a.GetTypes():
			attrsT = array(CmdClassAttribute, t.GetCustomAttributes(typeof(CmdClassAttribute), false))
			if attrsT != null and attrsT.Length > 0:
				for mi in t.GetMethods():
					attrsMi = array(CmdDeclarationAttribute,\
					   mi.GetCustomAttributes(CmdDeclarationAttribute, true))
					if attrsMi != null and attrsMi.Length > 0:
						descr=CmdDescr(attrsMi[0], attrsT[0], mi)
						self._collectedCmdsHelp.Add(descr)
						self._collectedCmds[attrsMi[0].Name]=descr
						for cmd in attrsMi[0].Shortcuts:
							self._collectedCmds[cmd] = descr
	public CollectedCmds:
		get:
			self.CollectCmds()
			return self._collectedCmds.Keys
	
	def GetCollectedCmd(nameOrShortcut):
		if string.IsNullOrEmpty(nameOrShortcut): return null
		result as CmdDescr
		self.CollectCmds()
		if self._collectedCmds.TryGetValue(nameOrShortcut, result):
			return result
		return null
	
	public CollectedCmdDecl:
		get:
			self.CollectCmds()
			return self._collectedCmdsHelp	
	
	public CollectedCmdModules:
		get:
			self.CollectCmds()
			result = Generic.HashSet of string()
			for cmd in self._collectedCmdsHelp:
				result.Add(cmd.Module.Name)
			return result
	
	property PreferShellCommands = false	
	def TurnOnPreferenceShellCommands():
		self.TogglePreferenceOnShellCommands() if not self.PreferShellCommands
	def TogglePreferenceOnShellCommands():
		self.PreferShellCommands = not self.PreferShellCommands
		if self.PreferShellCommands:
			Console.WriteLine("Shell commands will be preferred over BOO expressions.")
		else:
			Console.WriteLine("BOO expressions will be preferred over shell commands.") 
	
	def MaybeACommand(line as string):
	"""
	True iff shell commands are preferred or line starts
	with a slash.
	"""
		return self.PreferShellCommands and not line.StartsWith("/")
	
	def GetSuggestionsForCmdArg(query as string):
	"""
	Returns a string array of suggestions for the completion
	of a shell command argument or <c>null</c> if query is not
	a shell command.
	"""
		if not self.MaybeACommand(query): return null
		parsedCmd = CmdParser(query)
		cmd=self.GetCollectedCmd(parsedCmd.Cmd)
		if cmd == null: return null
		argIndex=0
		if len(parsedCmd.Args) > 0:
			if parsedCmd.LastArgClosed:
				argIndex = len(parsedCmd.Args)
				argQuery=string.Empty
				argPos = parsedCmd.EndPosArg[argIndex-1]
			else:
				argIndex = len(parsedCmd.Args)-1
				argQuery = parsedCmd.Args[argIndex]
				argPos = parsedCmd.StartPosArg[argIndex]
		else:
			argPos=len(parsedCmd.Cmd)+1
		cmdParams = cmd.Method.GetParameters()
		if argIndex >= len(cmdParams): return null
		cmdParam = cmdParams[argIndex]
		attrs=cmdParam.GetCustomAttributes(CmdArgumentAttribute, true) 
		if attrs == null or len(attrs) == 0:
			return null
		attr=attrs[0] as CmdArgumentAttribute
		if attr.Type == CmdArgumentCompletion.Directory:
			return (self.ReturnArgCompletionDirectory(argQuery), argPos)
		elif attr.Type == CmdArgumentCompletion.File:
			return (self.ReturnArgCompletionFile(string.Empty, argQuery), argPos)
		elif attr.Type == CmdArgumentCompletion.ExistingOrNotExistingFileOrExistingDirectory:
			return (self.ReturnArgCompletionFile('"', argQuery), argPos)
		elif attr.Type == CmdArgumentCompletion.Type:
			return (self.ReturnArgCompletionType(argQuery, null), argPos)
		elif attr.Type == CmdArgumentCompletion.TypeOrMember:
			return (self.ReturnArgCompletionExecutableTypeOrMember(argQuery), argPos)
		return null
	
	private def ReturnArgCompletionExecutableTypeOrMember(argQuery as string):
		if argQuery is null: argQuery = string.Empty
		lastComponentStart=argQuery.LastIndexOf('.')
		members = List of string()
		if lastComponentStart > 0:
			t=Type.GetType(argQuery[:lastComponentStart], false, false)
			memberNameFilter = argQuery[lastComponentStart+1:]
			if not t is null:
				for mi in t.GetMembers():
					if mi.Name.StartsWith(memberNameFilter, StringComparison.InvariantCultureIgnoreCase):
						members.Add("${t.Namespace}.${t.Name}.${mi.Name}")
		return ReturnArgCompletionType(argQuery, members)
	
	private def ReturnArgCompletionType(argQuery as string,\
			additionalResults as Collections.Generic.ICollection of string):
		if argQuery is null: argQuery = string.Empty
		lastComponentStart=argQuery.LastIndexOf('.')
		if lastComponentStart > 0:
			ns = Namespace.Find(argQuery[:lastComponentStart])
			argQuery=argQuery[lastComponentStart+1:]
		else:
			ns = Namespace.GetRootNamespace()
		result = List of string()
		if not ns is null:
			for cns in ns.Namespaces:
				if cns.Name.StartsWith(argQuery, StringComparison.InvariantCultureIgnoreCase):
					result.Add(cns.FullName+'.')
		if not additionalResults is null:
			result.AddRange(additionalResults)
		if not ns is null:
			for t in ns.Types:
				if t.IsVisible and t.IsPublic and t.Name.StartsWith(argQuery, StringComparison.InvariantCultureIgnoreCase):
					result.Add(repr(t))
		return result.ToArray()
	
	private def ReturnArgCompletionFile(dirQuote as string, argQuery as string):
		if string.IsNullOrWhiteSpace(argQuery):
			argQuery='.'
		argQuery=Path.GetFullPath(argQuery)
		result=List of string()
		parent=Path.GetDirectoryName(argQuery)
		if not string.IsNullOrEmpty(parent) and Directory.Exists(parent):
			result.Add('"'+parent+dirQuote)
		if Directory.Exists(argQuery):
			listDir=argQuery
			argQuery=string.Empty
		elif Directory.Exists(parent):
			listDir=parent
			argQuery=Path.GetFileName(argQuery)
		if not listDir is null:
			for f in Directory.GetFiles(listDir):
				if Path.GetFileName(f).StartsWith(argQuery, StringComparison.InvariantCultureIgnoreCase):
					result.Add('"'+f+'"')
			for d in Directory.GetDirectories(listDir):
				if Path.GetFileName(d).StartsWith(argQuery, StringComparison.InvariantCultureIgnoreCase):					
					result.Add('"'+d+dirQuote)
		return result.ToArray()
	
	private def ReturnArgCompletionDirectory(argQuery as string):
		if string.IsNullOrWhiteSpace(argQuery):
			argQuery='.'
		argQuery=Path.GetFullPath(argQuery)
		result=List of string()
		parent=Path.GetDirectoryName(argQuery)
		if not string.IsNullOrEmpty(parent) and Directory.Exists(parent):
			result.Add('"'+parent)
		if Directory.Exists(argQuery):
			result.Add('"'+argQuery+'"')
			for d in Directory.GetDirectories(argQuery):
				if not d.Equals(argQuery)\
					and d.StartsWith(argQuery, StringComparison.CurrentCultureIgnoreCase):
						result.Add('"'+d+'"')
		else:
			parent=Path.GetDirectoryName(argQuery)
			if not string.IsNullOrEmpty(parent) and Directory.Exists(parent):
				for d in Directory.GetDirectories(parent):
					if not d.Equals(argQuery)\
						and d.StartsWith(argQuery, StringComparison.CurrentCultureIgnoreCase):
							result.Add('"'+d+'"')
		return result.ToArray()
	
	def TryRunCommand(line as string):
	"""
	Run the buitin command as stated by a line string. Return false, if the
	line does not start a builtin command.
	Returns false if no command has been processed, true otherwise.
	"""  
		line=line.Trim()
		if "/".Equals(line):
			self.TogglePreferenceOnShellCommands()
			return true
		if not self.MaybeACommand(line):
			return false
		
		try:
			p=CmdParser(line)
			self.CollectCmds()
			cmdDescr as CmdDescr
			if self._collectedCmds.TryGetValue(p.Cmd, cmdDescr):
				instance=null
				if not cmdDescr.Method.IsStatic:
					if not self._cmdObjects.TryGetValue(cmdDescr.Method.DeclaringType.TypeHandle.Value.ToInt64(), instance):
						try:
							instance = cmdDescr.Method.DeclaringType.GetConstructor((InteractiveInterpreter,)).Invoke((self._interpreter as object,))
						except:
							instance = Activator.CreateInstance(cmdDescr.Method.DeclaringType)
						self._cmdObjects.Add(cmdDescr.Method.DeclaringType.TypeHandle.Value.ToInt64(), instance)
				cmdParameters = cmdDescr.Method.GetParameters()
				p.SetOnlyOneArgument() if cmdParameters.Length == 1
				args=[]
				for pi in cmdParameters:
					i = args.Count
					newArg=null
					pattrs = array(CmdArgumentAttribute, pi.GetCustomAttributes(CmdArgumentAttribute, false))
					if i < p.Args.Length:
						if i==cmdParameters.Length-1 and typeof(string).IsAssignableFrom(pi.ParameterType):
							collectedArgs=StringBuilder()
							for ii in range(i, p.Args.Length):
								if ii > i: collectedArgs.Append(' ')
								collectedArgs.Append(p.Args[i].ToString())
							args.Add(collectedArgs.ToString())
							break
						else:	
							newArg = Convert.ChangeType(p.Args[i], pi.ParameterType)
					else:
						if pattrs!= null and pattrs.Length > 0 and pattrs[0].DefaultValue != null:
							newArg = Convert.ChangeType(pattrs[0].DefaultValue, pi.ParameterType)
					if newArg == null:
						raise ApplicationException("Missing argument \"${pi.Name}\".")
					else:
						if pattrs!=null and pattrs.Length > 0:
							newArgStr=newArg.ToString()
							if (pattrs[0].Type & CmdArgumentCompletion.MaskPathName) != CmdArgumentCompletion.None\
								and newArgStr.StartsWith('"') and newArgStr.EndsWith('"'):
									newArg=newArgStr[1:-1]
						args.Add(newArg)
						/*
						elif pi.ParameterType.IsClass:
							args.Add(null)
						else:
							args.Add(Activator.CreateInstance(pi.ParameterType))
						*/
				cmdDescr.Method.Invoke(instance, args.ToArray())
				return true
		except exc:
			WithColor(ExceptionColor):
				while exc != null:
					Console.WriteLine(exc.Message)
					exc=exc.InnerException
			return true				
		return false
	
	static def PascalToUpperCaseSpelling(arg as string):
		result=string.Empty
		for i in range(arg.Length):
			c=arg[i]
			if i > 0 and char.IsUpper(c):
				result+="_"
			result+=string(c, 1)
		return result.ToUpper()
	
	public def DisplayHelp(filter as string):
	"""Displays helping information on the available shell command
	   on the Console."""
		if string.IsNullOrEmpty(filter):
			WithColor HelpHeadlineColor:
				Console.Write("Command Groups: ")
			WithColor HelpCmdColor:
				Console.WriteLine("Run help on one or more of this groups.")
			group = string.Empty
			for builtin in self._collectedCmdsHelp:
				if group != builtin.Module.Name:
					group = builtin.Module.Name
					WithColor InterpreterColor:
						Console.Write(' '+group)
			Console.WriteLine()
		else:
			filters = filter.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
			self.CollectCmds()
			group = string.Empty
			for builtin in self._collectedCmdsHelp:
				filterOK=false
				for f in filters:
					if builtin.Module.Name.StartsWith(f, StringComparison.InvariantCultureIgnoreCase):
						filterOK = true
						break
				if filterOK:
					if group != builtin.Module.Name:
						group = builtin.Module.Name
						WithColor HelpHeadlineColor:
							Console.WriteLine()
							Console.WriteLine(group+":")
					a=builtin.Descr
					line = a.Name
					if a.Shortcuts != null and a.Shortcuts.Length > 0:
						line+=" ("
						for i in range(a.Shortcuts.Length):
							line += ", " if i > 0
							line += a.Shortcuts[i]
						line +=")"
					params = builtin.Method.GetParameters()
					if not string.IsNullOrEmpty(a.Description) or (params != null and params.Length > 0):
						line+=":"
					WithColor HelpCmdColor:
						Console.Write("  ")
						Console.Write(line)
					params = builtin.Method.GetParameters()
					if params != null and params.Length > 0:
						line = " /"+a.Name+" "
						for p in params:
							if p.DefaultValue==null or string.IsNullOrEmpty(p.DefaultValue.ToString()):
								line += PascalToUpperCaseSpelling(p.Name)
							else:
								line += "["+PascalToUpperCaseSpelling(p.Name)+":"+p.DefaultValue+"]"
							line+=" "
						WithColor HelpTextBodyColor:
							Console.Write("  ")
							Console.Write(line)
					Console.WriteLine()
					if not string.IsNullOrEmpty(a.Description):
						WithColor HelpTextBodyColor:
							Console.WriteLine("    "+a.Description.Replace(Environment.NewLine, Environment.NewLine+"    "))
		
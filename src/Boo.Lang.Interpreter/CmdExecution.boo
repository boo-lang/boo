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
import System.Collections
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
		if self._collectedCmds.TryGetValue(nameOrShortcut, result):
			return result
		return null
	
	public CollectedCmdDecl:
		get:
			self.CollectCmds()
			return self._collectedCmdsHelp	
	
	public CollectedCmdModules:
		get:
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
		if not self.PreferShellCommands and not line.StartsWith("/"):
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
					newArg as string=null
					pattrs = array(CmdArgumentAttribute, pi.GetCustomAttributes(CmdArgumentAttribute, false))
					if i < p.Args.Length:
						newArg = Convert.ChangeType(p.Args[i], pi.ParameterType)
					else:
						if pattrs!= null and pattrs.Length > 0 and pattrs[0].DefaultValue != null:
							newArg = Convert.ChangeType(pattrs[0].DefaultValue, pi.ParameterType)
					if newArg == null:
						raise ApplicationException("Missing argument ${pi.Name}.")
					else:
						if pattrs!=null and pattrs.Length > 0:
							if (pattrs[0].Type & CmdArgumentCompletion.MaskPathName) != CmdArgumentCompletion.None\
								and newArg.StartsWith('"') and newArg.EndsWith('"'):
									newArg=newArg[1:-1]
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
		
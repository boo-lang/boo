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

import System
import System.Collections.Generic
import System.Diagnostics
import System.IO
import Boo.Lang.Interpreter

static class Options:
	public console = InteractiveInterpreterConsole()
	public nologo = false
	public loadRequests = System.Collections.Generic.List[of string]()
	public importRequests = System.Collections.Generic.List[of string]()
	public debugOrWarnings=false
	public exit=false

	def Read(options as IEnumerable[of string]):
		for arg in options:
			if arg == "--help" or arg == "-help" or arg == "-h":
				PrintOptions()
				exit=true
			if arg == "--print-modules" or arg == "-print-modules":
				console.PrintModules = true
			if arg == "--debug" or arg == "-debug" or arg == "-d":
				debugOrWarnings=true
				Debug.Listeners.Add(TextWriterTraceListener(Console.Out))
			if arg == "-w":
				debugOrWarnings=true
				console.ShowWarnings = true
			if arg.StartsWith("-r:"):
				loadRequests.Add(arg.Substring(3))
			if arg.StartsWith("-i:"):
				if arg.Contains(","):
					args=arg.Substring(3).Split(","[0])
					importRequests.Add("import "+args[0]+" from "+args[1])
				else:
					importRequests.Add("import "+arg.Substring(3))
			if arg == "--nologo" or arg == "-nologo":
				nologo = true
			if not arg.StartsWith("-"):
				loadRequests.Add(arg)
		
	def PrintOptions():
		print "booish [OPTIONS] [BooFile]*"
		print "-h/-help/--help        Print this information"
		print "-d/-debug/--debug      Turn on a mode printing debug information"
		print "-w                     Turn on mode showing warnings"
		print "-r:BooFile             Interpret a BOO file on start"
		print "-r:AssemblyFile        Add reference to this assembly"
		print "-r:AssemblyPartialName Add reference to this assembly"
		print "-i:Namespace           Import a namespace"
		print "-i:Namespace Assembly  Import a namespace from an assembly"

Options.Read(argv)
return if Options.exit

rspBaseName=Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName)+'.rsp'
rspPath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rspBaseName)

if File.Exists(rspPath):
	print "Options in ${rspBaseName}:" if Options.debugOrWarnings
	for rspLine in File.OpenText(rspPath).ReadToEnd().Split(*"\n\r".ToCharArray()):
		if not rspLine.StartsWith('#') and not rspLine.StartsWith('//'):
			for rspOpt in rspLine.Split(*" ".ToCharArray()):
				if not string.IsNullOrEmpty(rspOpt):
					print rspOpt if Options.debugOrWarnings
					Options.Read((rspOpt,))
					return if Options.exit
	print

Options.console.DisplayLogo() unless Options.nologo

for req in Options.loadRequests:
	Options.console.Load(req) if not string.IsNullOrEmpty(req)
for req in Options.importRequests:
	print req
	Options.console.Eval(req)

Options.console.Eval("import Boo.Lang.Interpreter.Builtins")
Options.console.ReadEvalPrintLoop()

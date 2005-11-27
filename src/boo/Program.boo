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
import System.IO
import System.Text
import Boo.Lang.Useful.CommandLine
import Boo.Lang.Interpreter
		
class Program:
	
	_cmdLine = BooCommandLine()
	
	def main(argv as (string)):
		
		error as CommandLineException
		try:
			_cmdLine.Parse(argv)
		except x as CommandLineException:
			error = x
	
		processing = do:
			if (error is not null
				or _cmdLine.Help
				or not _cmdLine.IsValid):
				print error.Message if error is not null
				usage()
				return -1		
			process()
			
		if _cmdLine.UTF8:
			using writer = StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8):
				Console.SetOut(writer)
				Console.WriteLine()
				return processing()
		else:
			return processing()

	def banner():
		print ResourceManager.GetString("boo.CommandLine.header"), BooVersion
		print
		
	def usage():
		banner()
		print ResourceManager.GetString("boo.CommandLine.usage")
		_cmdLine.PrintOptions()
	
	def process():
		banner()
#		given _cmdLine.Style:
#			when BooCommandLineStyle.InteractiveInterpreter:
#				booish()
#			when BooCommandLineStyle.Interpreter:
#				booi()
#			when BooCommandLineStyle.Compiler:
#				booc()
#			otherwise:
#				assert false
		if _cmdLine.Style == BooCommandLineStyle.InteractiveInterpreter:
			booish()
		elif _cmdLine.Style == BooCommandLineStyle.Interpreter:
			booi()
		else:
			booc()

	def booc():
		pass
			
	def booi():
		pass
	
	def booish():
		print """The following builtin functions are available:
	dir(Type): lists the members of a type
	help(Type): prints detailed information about a type
	load(string): evals an external boo file
	globals(): returns the names of all variables known to the interpreter

Enter boo code in the prompt below."""
		interpreter = InteractiveInterpreter(RememberLastValue: true)
		interpreter.ConsoleLoopEval()

[STAThread]
def Main(argv as (string)):
	return Program().main(argv)


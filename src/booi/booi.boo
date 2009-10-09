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

namespace booi

import System
import System.IO
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Useful.Attributes

class Program:
	
	public static final DefaultErrorCode = 127
	public static final DefaultSuccessCode = 0
	
	_cmdLine as (string)
	_assemblyResolver = AssemblyResolver()
	_compiler = BooCompiler()
	_printWarnings = false
	
	def constructor(cmdLine as (string)):
		_cmdLine = cmdLine
		_compiler.Parameters.Pipeline = CompileToMemory()
	
	def run():
		installAssemblyResolver()
		consumedArguments = processCommandLine()		
		assembly = compile()	
		if assembly is null: return DefaultErrorCode
		return execute(assembly, _cmdLine[consumedArguments:])
		
	def compile():
		result = _compiler.Run()
		if _printWarnings and len(result.Warnings):
			print(result.Warnings.ToString())
		if len(result.Errors):
			print(result.Errors.ToString(true))
			return null
		return result.GeneratedAssembly
		
	def execute(generatedAssembly as Assembly, argv as (string)):
		
		Environment.ExitCode = exitCode = DefaultSuccessCode

		try: 
			_assemblyResolver.AddAssembly(generatedAssembly)
			main = generatedAssembly.EntryPoint
			if len(main.GetParameters()) > 0:
				returnValue = main.Invoke(null, (argv,))
			else:
				returnValue = main.Invoke(null, null)
			exitCode = returnValue if returnValue is not null
		except x as TargetInvocationException:
			print(x.InnerException)
			exitCode = DefaultErrorCode
		ensure:
			Environment.ExitCode = exitCode
			
		return exitCode
		
	def processCommandLine():
		consumedArgs = 1
		for arg in _cmdLine:
			if "-" == arg:
				_compiler.Parameters.Input.Add(StringInput("<stdin>", consume(Console.In)))
				break
			elif "-ducky" == arg:
				_compiler.Parameters.Ducky = true
			elif "-wsa" == arg:
				_compiler.Parameters.Pipeline[0] = Boo.Lang.Parser.WSABooParsingStep()
			elif "-w" == arg:
				_printWarnings = true
			elif arg.StartsWith("-r:"):
				try:
					asm = _compiler.Parameters.LoadAssembly(arg[3:])
					if asm is null:
						print Boo.Lang.ResourceManager.Format("BooC.UnableToLoadAssembly", arg[3:])
					else:
						_compiler.Parameters.References.Add(asm)
						_assemblyResolver.AddAssembly(asm)
				except e:
					print e.Message
			else:
				_compiler.Parameters.Input.Add(FileInput(translatePath(arg)))
				break
			++consumedArgs
		return consumedArgs
		
	def translatePath(path as string):
		if isCygwin(): 	return translateCygwinPath(path)
		return path
		
	[once]
	def isCygwin():
		return Environment.GetEnvironmentVariable("TERM") == "cygwin"
	
	def translateCygwinPath(path as string):
		if not path.StartsWith("/"): return path
		if path.StartsWith("/cygdrive"):
			path = path[len("/cygdrive/"):]
			driveLetter = path[0]
			return driveLetter + ":" + path[1:]
		return Path.Combine(cygwinRoot(), path[1:])
		
	[once]
	def cygwinRoot():
		home = Environment.GetEnvironmentVariable("HOME")
		assert home is not null
		return home[:home.IndexOf("\\home")]
		
	def installAssemblyResolver():
		AppDomain.CurrentDomain.AssemblyResolve += _assemblyResolver.AssemblyResolve

def consume(reader as TextReader):
	return join(line for line in reader, "\n")

[STAThread]
def Main(argv as (string)) as int:
	
	if len(argv) < 1:
		print("booi <script.boo>") 
		return Program.DefaultErrorCode
		
	return Program(argv).run();
	
	


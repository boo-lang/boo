#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import System
import System.IO
import System.Reflection
import System.Threading
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class AssemblyResolver:

	_cache = {}
	
	def AddAssembly([required] asm as Assembly):
		_cache[GetSimpleName(asm.FullName)] = asm
	
	def AssemblyResolve(sender, args as ResolveEventArgs) as Assembly:		
		simpleName = GetSimpleName(args.Name)
		
		asm as Assembly = _cache[simpleName]
		if asm is null:
			basePath = Path.GetFullPath(simpleName)
			asm = ProbeFile(basePath + ".dll")
			if asm is null:
				asm = ProbeFile(basePath + ".exe")
			_cache[simpleName] = asm
			
		return asm
		
	private def GetSimpleName(name as string):
		return /,\s*/.Split(name)[0]
		
	private def ProbeFile(fname as string):	
		return Assembly.LoadFrom(fname) if File.Exists(fname)

def consume(reader as TextReader):
	writer = StringWriter()
	for line in reader:
		writer.WriteLine(line)
	return writer.ToString()

def main(argv as (string)):
	Thread.CurrentThread.ApartmentState = ApartmentState.STA
	
	compiler = BooCompiler()
		
	// boo memory pipeline
	// compiles the code in memory only
	compiler.Parameters.Pipeline = CompileToMemory()	
	
	if "-" == argv[0]:
		compiler.Parameters.Input.Add(StringInput("<stdin>", consume(Console.In)))
	else:
		compiler.Parameters.Input.Add(FileInput(argv[0]))
	
	resolver = AssemblyResolver()
	AppDomain.CurrentDomain.AssemblyResolve += resolver.AssemblyResolve
	result = compiler.Run()
	if len(result.Errors):
		for error in result.Errors:
			print(error.ToString(true))
		return -1
	else:	
		try: 
			resolver.AddAssembly(result.GeneratedAssembly)
			result.GeneratedAssemblyEntryPoint.Invoke(null, (argv[1:],))			
		except x as TargetInvocationException:
			print(x.InnerException)
			return -1
	return 0
	
if len(argv) > 0:
	Environment.Exit(main(argv))
else:
	print("booi <script.boo>")

#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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

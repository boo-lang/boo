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
import System.Reflection
import System.Security.Permissions
import System.Threading
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class AssemblyResolver:

	_cache = {}
	
	def AddAssembly([required] asm as Assembly):
		_cache[GetSimpleName(asm.FullName)] = asm
		
	def LoadAssembly([required] name as string):
		asm = ProbeFile(name)
		if asm is not null:
			_cache[asm.GetName().Name] = asm
		return asm
	
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
	return join(line for line in reader, "\n")

[STAThread]
def Main(argv as (string)):
	
	if len(argv) < 1:
		print("booi <script.boo>") 
		return -1
	
	resolver = AssemblyResolver()
	AppDomain.CurrentDomain.AssemblyResolve += resolver.AssemblyResolve
	
	compiler = BooCompiler()
	compiler.Parameters.Pipeline = CompileToMemory()
	
	consumedArgs = 1
	asm as Assembly = null
	for arg in argv:
		if "-" == arg:
			compiler.Parameters.Input.Add(StringInput("<stdin>", consume(Console.In)))
			break
		elif "-ducky" == arg:
			compiler.Parameters.Ducky = true
		elif "-w" == arg:
			printWarnings = true
		elif arg.StartsWith("-r:"):			
			//compiler.Parameters.References.Add(resolver.LoadAssembly(arg[3:]))
			try:
				asm = compiler.Parameters.LoadAssembly(arg[3:])
				if asm is null:
					print Boo.Lang.ResourceManager.Format("BooC.UnableToLoadAssembly", arg[3:])
				else:
					compiler.Parameters.References.Add(asm)
					resolver.AddAssembly(asm)
			except e:
				print e.Message
		else:
			compiler.Parameters.Input.Add(FileInput(arg))
			break
		++consumedArgs
	
	result = compiler.Run()
	if printWarnings and len(result.Warnings):
		print(result.Warnings.ToString())
	if len(result.Errors):
		print(result.Errors.ToString(true))
		return -1
	else:	
		try: 
			resolver.AddAssembly(result.GeneratedAssembly)
			result.GeneratedAssembly.EntryPoint.Invoke(null, (argv[consumedArgs:],))			
		except x as TargetInvocationException:
			print(x.InnerException)
			return -1
	return 0
	
[assembly: SecurityPermission(
						SecurityAction.RequestMinimum,
						ControlAppDomain: true)] 
	


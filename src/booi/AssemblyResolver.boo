namespace booi

import System
import System.Reflection
import System.IO

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

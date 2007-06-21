#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

import System
import System.IO
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

class AssemblyResolver:

	_cache = {}
	
	def AssemblyResolve(sender, args as ResolveEventArgs) as Assembly:
		parts = /,\s*/.Split(args.Name)
		simpleName = parts[0]
		
		asm as Assembly = _cache[simpleName]
		if asm is null:
			basePath = Path.GetFullPath(simpleName)
			asm = probeFile(basePath + ".dll")
			if asm is null:
				asm = probeFile(basePath + ".exe")
			_cache[simpleName] = asm
			
		return asm

def main(argv as (string)):
	compiler = BooCompiler()
	if "-" == argv[0]:
		compiler.Parameters.Input.Add(ReaderInput("<stdin>", System.Console.In))
	else:
		compiler.Parameters.Input.Add(FileInput(argv[0]))
		
	// boo memory pipeline
	// compiles the code in memory only
	compiler.Parameters.Pipeline.Load("boom")
	
	AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver().AssemblyResolve
	result = compiler.Run()
	if len(result.Errors):
		for error as CompilerError in result.Errors:
			print(error.ToString(true))
	else:	
		try:
			result.GeneratedAssemblyEntryPoint.Invoke(null, (argv[1:],))
		except x as TargetInvocationException:
			print(x.InnerException)
	
def probeFile(fname as string):	
	if File.Exists(fname):		
		return Assembly.LoadFrom(fname)

	
if len(argv) > 0:
	main(argv)
else:
	print("booi <script.boo>")

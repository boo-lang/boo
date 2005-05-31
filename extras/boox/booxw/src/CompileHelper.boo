#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
//
// This file is part of Boo Explorer.
//
// Boo Explorer is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Boo Explorer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooExplorer

import System.Text.RegularExpressions
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

class CompiledScript:
	_ctx as CompilerContext
	
	Errors as CompilerErrorCollection:
		get:
			return _ctx.Errors

	def constructor(ctx as CompilerContext):
		_ctx = ctx
				
	def Execute():
		return if len(_ctx.Errors)
		_ctx.GeneratedAssembly.EntryPoint.Invoke(null, (null,))
		
	def GetType(typeName as string):
		return null if len(_ctx.Errors)
		return _ctx.GeneratedAssembly.GetType(typeName)
	
	def GetTypes(match as string):
		return null if len(_ctx.Errors)
		return [t for t in _ctx.GeneratedAssembly.GetTypes() if t.Name =~ Regex(match)]
		
	
	def GetTypes():
		return null if len(_ctx.Errors)
		return _ctx.GeneratedAssembly.GetTypes()

class ScriptCompiler:
	static def CompileFile([required] fileName as string) as CompiledScript:
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(FileInput(fileName))
		compiler.Parameters.Pipeline = CompileToMemory()
		compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
		compiler.Parameters.OutputType = CompilerOutputType.Library
		return CompiledScript(compiler.Run())

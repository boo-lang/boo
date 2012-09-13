#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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


namespace Boo.Lang.Interpreter.Builtins

import System
import System.Collections
import System.IO

def repr(value):
	return Repr.repr(value)

static class Repr:
	
	_representers = []
	
	def repr(value):
		writer = System.IO.StringWriter()
		repr(value, writer)
		return writer.ToString()
	
	def repr(value, writer as System.IO.TextWriter):
		if value is null: return
		InitializeRepresenters() if 0 == len(_representers)
		GetBestRepresenter(value.GetType())(value, writer)

	private def InitializeRepresenters():
		AddRepresenter(string) do (value as string, writer as TextWriter):
			Boo.Lang.Compiler.Ast.Visitors.BooPrinterVisitor.WriteStringLiteral(value, writer)
			
		AddRepresenter(bool) do (value as bool, writer as TextWriter):
			writer.Write(("true" if value else "false"))
			
		AddRepresenter(Array) do (a as Array, writer as TextWriter):
			writer.Write("(")
			RepresentItems(a, writer)
			writer.Write(")")
				
		AddRepresenter(Delegate) do (d as Delegate, writer as TextWriter):
			method = d.Method
			if method.DeclaringType is not null:
				writer.Write(method.DeclaringType.FullName)
				writer.Write(".")
			writer.Write(method.Name)
		
		AddRepresenter(IDictionary) do (value as IDictionary, writer as TextWriter):
			writer.Write("{")
			i = 0
			for key in value.Keys:
				writer.Write(", ") if i
				repr(key, writer)
				writer.Write(": ")
				repr(value[key], writer)
				++i
			writer.Write("}")
			
		AddRepresenter(IList) do (value as IList, writer as TextWriter):
			writer.Write("[")
			RepresentItems(value, writer)
			writer.Write("]")
			
		AddRepresenter(long) do (value, writer as TextWriter):
			writer.Write(value)
			writer.Write("L")
				
		AddRepresenter(object) do (value, writer as TextWriter):
			writer.Write(value)
			
	private def RepresentItems(items, writer as TextWriter):
		i = 0
		for item in items:
			writer.Write(", ") if i > 0				
			repr(item, writer)
			++i
			
	private def AddRepresenter(type as Type, value as Action[of object, TextWriter]):
		_representers.Add((type, value))
		
	def GetBestRepresenter(type as Type) as Action[of object, TextWriter]:
		for key as Type, value in _representers:
			return value if key.IsAssignableFrom(type)
		raise ArgumentException("An appropriate representer could not be found!")

	


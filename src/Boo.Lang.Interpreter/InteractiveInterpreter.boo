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

namespace Boo.Lang.Interpreter

import System
import System.Collections
import System.IO
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO

class InteractiveInterpreter(AbstractInterpreter):

	_values = {}
	
	_declarations = {}
	
	_representers = []
	
	[property(ShowWarnings)]
	_showWarnings = false
	
	[property(BlockStarters, value is not null)]
	_blockStarters = ":", "\\"
	
	[getter(LastValue)]
	_lastValue = null
	
	[property(Print, value is not null)]
	_print as Action[of object] = print
	
	def constructor():
		super()
		InitializeStandardReferences()
		
	def constructor(parser as ICompilerStep):
		super(parser)
		InitializeStandardReferences()
		
	def Reset():
		_values.Clear()
		_declarations.Clear()
		_lastValue = null
		InitializeStandardReferences()
		
	def ConsoleLoopEval():			
		while (line=prompt(">>> ")) is not null:
			try:		
				line = ReadBlock(line) if line[-1:] in _blockStarters
				InternalLoopEval(line)
			except x as System.Reflection.TargetInvocationException:
				print(x.InnerException)
			except x:
				print(x)
		print
		
	def LoopEval(code as string):
		using console = ConsoleCapture():
			result = InternalLoopEval(code)
			for line in System.IO.StringReader(console.ToString()):
				_print(line)
		return result
			
	private def InternalLoopEval(code as string):
		result = self.Eval(code)
		if ShowWarnings:
			self.DisplayProblems(result.Warnings)
		if not self.DisplayProblems(result.Errors):
			ProcessLastValue()
		return result
		
	private def ProcessLastValue():
		_ = self.LastValue
		if _ is not null:
			_print(repr(_))
			SetValue("_", _)
	
	override def Declare([required] name as string,
				[required] type as System.Type):
		_declarations.Add(name, type)
		
	override def SetLastValue(value):
		_lastValue = value
		
	override def SetValue(name as string, value):
		_values[name] = value
		return value

	override def GetValue(name as string):
		return _values[name]
		
	override def Lookup([required] name as string):
		type as System.Type = _declarations[name]
		return type if type is not null
		
		value = GetValue(name)
		return value.GetType() if value is not null
	
	def DisplayProblems(problems as ICollection):
		return if problems is null or problems.Count == 0
		for problem as duck in problems:
			markLocation(problem.LexicalInfo)
			type = ("WARNING", "ERROR")[problem isa CompilerError]
			_print("${type}: ${problem.Message}")
		if problems.Count > 0:
			return true
		return false

	private def markLocation(location as LexicalInfo):
		pos = location.Column
		_print("---" + "-" * pos + "^") if pos > 0
		
	private def InitializeStandardReferences():
		SetValue("interpreter", self)
		SetValue("dir", dir)
		SetValue("help", help)
		SetValue("print", { value | _print(value) })
		SetValue("load", load)
		SetValue("globals", globals)
		SetValue("getRootNamespace", Namespace.GetRootNamespace)
		
	def globals():
		return array(string, _values.Keys)
					
	def dir([required] obj):
		type = (obj as Type) or obj.GetType()
		return array(
				member for member in type.GetMembers()
				unless (method=(member as System.Reflection.MethodInfo))
				and method.IsSpecialName)
				
	def load([required] path as string):
		if path.EndsWith(".boo"):
			result = EvalCompilerInput(FileInput(path))
			if ShowWarnings:
				DisplayProblems(result.Warnings)
			if not DisplayProblems(result.Errors):
				ProcessLastValue()			
		else:
			try:
				References.Add(System.Reflection.Assembly.LoadFrom(path))
			except e:				
				print e.Message
		
	def help(obj):		
		type = (obj as Type) or obj.GetType()
		for line in Help.HelpFormatter("    ").GenerateFormattedLinesFor(type):
			_print(line)
		
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
			Visitors.BooPrinterVisitor.WriteStringLiteral(value, writer)
			
		AddRepresenter(bool) do (value as bool, writer as TextWriter):
			writer.Write(("false", "true")[value])
			
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
				
		AddRepresenter(object) do (value, writer as TextWriter):
			writer.Write(value)
			
	private def RepresentItems(items, writer as TextWriter):
		i = 0
		for item in items:
			writer.Write(", ") if i > 0				
			repr(item, writer)
			++i
			
	callable Representer(value, writer as TextWriter)
	
	private def AddRepresenter(type as Type, value as Representer):
		_representers.Add((type, value))
		
	def GetBestRepresenter(type as Type) as Representer:
		for key as Type, value in _representers:
			return value if key.IsAssignableFrom(type)
		assert false, "An appropriate representer could not be found!"

def ReadBlock(line as string):
	newLine = System.Environment.NewLine
	buffer = System.Text.StringBuilder()
	buffer.Append(line)
	buffer.Append(newLine)
	while line=prompt("... "):
		break if 0 == len(line)
		buffer.Append(line)
		buffer.Append(newLine)
	return buffer.ToString()

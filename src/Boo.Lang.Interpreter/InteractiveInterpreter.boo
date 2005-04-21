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
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.IO

class InteractiveInterpreter(AbstractInterpreter):

	_values = {}
	
	_declarations = {}
	
	_representers = []
	
	[getter(LastValue)]
	_lastValue
	
	[property(Print, value is not null)]
	_print as callable(object) = print
	
	def constructor():
		super()
		InitializeStandardReferences()
		
	def Reset():
		_values.Clear()
		_declarations.Clear()
		_lastValue = null
		InitializeStandardReferences()
		
	def ConsoleLoopEval():			
		while line=prompt(">>> "):
			try:		
				line = ReadBlock(line) if line[-1:] in ":", "\\"
				LoopEval(line)
			except x as System.Reflection.TargetInvocationException:
				print(x.InnerException)
			except x:
				print(x)
		print
			
	def LoopEval(code as string):
		result = self.Eval(code)
		if len(result.Errors):
			self.DisplayErrors(result.Errors)
		else:
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
		
	def DisplayErrors(errors as CompilerErrorCollection):
		for error in errors:
			pos = error.LexicalInfo.Column
			_print("---" + "-" * pos + "^") if pos > 0
			_print("ERROR: ${error.Message}")
		
	private def InitializeStandardReferences():
		SetValue("interpreter", self)
		SetValue("dir", dir)
		SetValue("help", help)
		SetValue("print", { value | _print(value) })
		SetValue("load", load)
		SetValue("globals", globals)
		
	def globals():
		return array(string, _values.Keys)
					
	def dir([required] obj):
		type = (obj as Type) or obj.GetType()
		return array(
				member for member in type.GetMembers()
				unless (method=(member as System.Reflection.MethodInfo))
				and method.IsSpecialName)
				
	def load([required] path as string):
		References.Add(System.Reflection.Assembly.LoadFrom(path))
		
	def help(obj):		
		type = (obj as Type) or obj.GetType()
		DescribeType("    ", type)
		
	def DescribeType(indent as string, type as Type):
		
		if type.IsInterface:
			typeDef = "interface"
			baseTypes = array(GetBooTypeName(t) for t in type.GetInterfaces())
		else:
			typeDef = "class"
			baseTypes = (GetBooTypeName(type.BaseType),) + array(GetBooTypeName(t) for t in type.GetInterfaces())
			
		_print("${typeDef} ${type.Name}(${join(baseTypes, ', ')}):")
		_print("")
		
		for ctor in type.GetConstructors():
			_print("${indent}def constructor(${DescribeParameters(ctor.GetParameters())})")
			_print("")
			
		sortByName = def (lhs as Reflection.MemberInfo, rhs as Reflection.MemberInfo):
			return lhs.Name.CompareTo(rhs.Name)
			
		for f as Reflection.FieldInfo in List(type.GetFields()).Sort(sortByName):
			_print("${indent}public ${DescribeField(f)}")
			_print("")
			
		for p as Reflection.PropertyInfo in List(type.GetProperties()).Sort(sortByName):
			_print("${indent}${DescribeProperty(p)}:")
			_print("${indent}${indent}get") if p.GetGetMethod() is not null
			_print("${indent}${indent}set") if p.GetSetMethod() is not null
			_print("")		
		
		for m as Reflection.MethodInfo in List(type.GetMethods()).Sort(sortByName):
			continue if m.IsSpecialName
			_print("${indent}${DescribeMethod(m)}")
			_print("")
			
		for e as Reflection.EventInfo in List(type.GetEvents()).Sort(sortByName):
			_print("${indent}${DescribeEvent(e)}")
			_print("")
			
	static def DescribeEvent(e as Reflection.EventInfo):
		return "${DescribeModifiers(e)}event ${e.Name} as ${e.EventHandlerType}"
			
	static def DescribeProperty(p as Reflection.PropertyInfo):
		modifiers = DescribeModifiers(p)
		params = DescribePropertyParameters(p.GetIndexParameters())
		return "${modifiers}${p.Name}${params} as ${GetBooTypeName(p.PropertyType)}"
			
	static def DescribeField(f as Reflection.FieldInfo):
		return "${DescribeModifiers(f)}${f.Name} as ${GetBooTypeName(f.FieldType)}"
			
	static def DescribeMethod(m as Reflection.MethodInfo):
		returnType = GetBooTypeName(m.ReturnType)
		modifiers = DescribeModifiers(m)
		return "${modifiers}def ${m.Name}(${DescribeParameters(m.GetParameters())}) as ${returnType}"
			
	static def DescribeModifiers(f as Reflection.FieldInfo):
		return "static " if f.IsStatic
		return ""
			
	static def DescribeModifiers(m as Reflection.MethodBase):
		return "static " if m.IsStatic
		return ""
		
	static def DescribeModifiers(e as Reflection.EventInfo):
		return DescribeModifiers(e.GetAddMethod() or e.GetRemoveMethod())
		
	static def DescribeModifiers(p as Reflection.PropertyInfo):
		accessor = p.GetGetMethod() or p.GetSetMethod()
		return DescribeModifiers(accessor)
			
	static def DescribePropertyParameters(parameters as (Reflection.ParameterInfo)):
		return "" if 0 == len(parameters)
		return "(${DescribeParameters(parameters)})"
			
	static def DescribeParameters(parameters as (Reflection.ParameterInfo)):
		return join(DescribeParameter(p) for p in parameters, ", ")
		
	static def DescribeParameter(p as Reflection.ParameterInfo):
		return "${p.Name} as ${GetBooTypeName(p.ParameterType)}"
		
	static def GetBooTypeName(type as System.Type):
		return "(${GetBooTypeName(type.GetElementType())})" if type.IsArray
		return "object" if object is type
		return "string" if string is type
		return "void" if void is type
		return "bool" if bool is type		
		return "byte" if byte is type
		return "char" if char is type
		return "sbyte" if sbyte is type
		return "short" if short is type
		return "ushort" if ushort is type
		return "int" if int is type
		return "uint" if uint is type
		return "long" if long is type
		return "ulong" if ulong is type
		return "single" if single is type
		return "double" if double is type
		return "date" if date is type
		return "timespan" if timespan is type
		return "regex" if regex is type
		return type.FullName
		
	def repr(value):
		writer = System.IO.StringWriter()
		repr(value, writer)
		return writer.ToString()
	
	def repr(value, writer as System.IO.TextWriter):
		return unless value is not null
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
			writer.Write(method.DeclaringType.FullName)
			writer.Write(".")
			writer.Write(method.Name)
		
		AddRepresenter(IDictionary) do (value as IDictionary, writer as TextWriter):
			writer.Write("{")
			i = 0
			for item as DictionaryEntry in value:
				writer.Write(", ") if i
				repr(item.Key, writer)
				writer.Write(": ")
				repr(item.Value, writer)
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

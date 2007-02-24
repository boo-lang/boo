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
import System.Text
import System.Text.RegularExpressions
import System.Reflection
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.IO


class InteractiveInterpreter2(AbstractInterpreter):

	_values = {}
	
	_declarations = {}
	
	_representers = []
	
	[property(ShowWarnings)]
	_showWarnings = false
	
	[property(BlockStarters, value is not null)]
	_blockStarters = ":", "\\"
	
	[getter(LastValue)]
	_lastValue
	
	[property(Print, value is not null)]
	_print as callable(object) = print
	
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
	
	private _buffer = StringBuilder()
	
	
	# color for messages from the interpreter (not from user code)
	[property(InterpreterColor)]
	_interpreterColor = ConsoleColor.Gray
	
	[property(PromptColor)]
	_promptColor = ConsoleColor.Green
	
	[property(ExceptionColor)]
	_exceptionColor = ConsoleColor.Red

	[property(SuggestionsColor)]
	_suggestionsColor = ConsoleColor.Yellow

	[property(SelectedSuggestionColor)]
	_selectedSuggestionColor = ConsoleColor.Magenta

	[property(SelectedSuggestionIndex)]
	_selectedSuggestionIndex = -1
	
	[property(Suggestions)]
	_suggestions as (object)
	
	_builtins as (IEntity)
	
	_filter as string
	
	CanAutoComplete as bool:
		get:
			return _selectedSuggestionIndex >= 0 and _suggestions is not null
	
	private def ConsolePrintPrompt():
		Console.ForegroundColor = _promptColor
		if _indent == 0:
			Console.Write(">>>")
		else:
			Console.Write("...")
			#TODO: automatic indentation option ?
			#for i in range(_indent):
			#	_buffer.Append("\t")
			#	Console.Write("\t")			
		Console.ResetColor()
	
	private def ConsolePrintException(e as Exception):
		Console.ForegroundColor = _exceptionColor
		print e
		Console.ResetColor()


	protected def ConsolePrintSuggestions():
		cursorLeft = Console.CursorLeft
		cursorTop = Console.CursorTop
		Console.Write(Environment.NewLine)
		
		i = 0

		if _suggestions isa (string):
			for l in _suggestions as (string):
				Console.ForegroundColor = _suggestionsColor
				if i > 0:				
					Console.Write(", ")
				if i == _selectedSuggestionIndex:
					Console.ForegroundColor = _selectedSuggestionColor			
				Console.Write(l)
				i++	

		elif _suggestions isa (IEntity):
			for e in _suggestions as (IEntity):
				Console.ForegroundColor = _suggestionsColor
				if i > 0:				
					Console.Write(", ")
				if i == _selectedSuggestionIndex:
					Console.ForegroundColor = _selectedSuggestionColor			
				Console.Write(DescribeEntity(e))
				i++
		
		Console.ResetColor()
		Console.CursorLeft = cursorLeft
		Console.CursorTop = cursorTop
		Console.Write(Environment.NewLine)
		ConsolePrintPrompt()
		Console.Write(_buffer.ToString())


	private static re_open = Regex("\\(", RegexOptions.Singleline)
	private static re_close = Regex("\\)", RegexOptions.Singleline)

	def DisplaySuggestions(query as string):
		#TODO: FIXME: refactor to one regex?
		p_open = re_open.Matches(query).Count
		p_close = re_close.Matches(query).Count
		if p_open > p_close:
			query = query.Split(" ,(\t".ToCharArray(), 100)[-1]
		else:
			query = query.Split(" ,\t".ToCharArray(), 100)[-1]
		if query.LastIndexOf('.') > 0:
			codeToComplete = query[0:query.LastIndexOf('.')+1]
			_suggestions = SuggestCodeCompletion(codeToComplete+"__codecomplete__")
			_filter = query[query.LastIndexOf('.')+1:]
			_suggestions = array(e for e in _suggestions as (IEntity)
								unless not e.Name.StartsWith(_filter)) as (IEntity)			

		if not _suggestions or 0 == len(_suggestions): #suggest a  var		
			_filter = query
			_suggestions = array(var.ToString() for var in _values.Keys
									unless not var.ToString().StartsWith(_filter))
			
		if not _suggestions or 0 == len(_suggestions):
			_selectedSuggestionIndex = -1
			#Console.Beep() #TODO: flash background?
		elif 1 == len(_suggestions):
			AutoComplete()
		else:
			ConsolePrintSuggestions()

		
	def AutoComplete():
		if not _suggestions:
			raise InvalidOperationException("no suggestions")
		
		s = _suggestions[_selectedSuggestionIndex] as IEntity
		if s:
			if s.EntityType == EntityType.Method:
				m = s as IMethod		
				completion = s.Name[len(_filter):]
				Console.Write(completion)
				_buffer.Append(completion)
				if not m.GetParameters() or m.GetParameters().Length == 0:
					Console.Write('()')
					_buffer.Append('()')
				else:
					Console.Write('(')
					_buffer.Append('(')
			
			else: # not a Method
				completion = s.Name[len(_filter):]
				Console.Write(completion)
				_buffer.Append(completion)
		else:
			completion = (_suggestions[_selectedSuggestionIndex] as string)[len(_filter):]
			Console.Write(completion)
			_buffer.Append(completion)			
			
		_selectedSuggestionIndex = -1
		_suggestions = null
	
	
	def ConsoleLoopEval():
		Console.CursorVisible = true
		ConsolePrintPrompt()
		
		lastChar = char('0')
		key = ConsoleKey.LeftArrow		
		while key != ConsoleKey.Escape:						
			cki = Console.ReadKey(true) #TODO: fix mono different behavior when ()
			key = cki.Key
			keyChar = cki.KeyChar
			control = false
			
			newLine = keyChar in Environment.NewLine

			if char.IsControl(keyChar):
				control = true
				if keyChar == char('\t'):
					test = char(' ')
					test = _buffer.ToString()[_buffer.Length-1] if _buffer.Length > 0
					if char.IsLetterOrDigit(test) or test == char('.'): 
						_selectedSuggestionIndex = 0
						DisplaySuggestions(_buffer.ToString())					
					else:
						_buffer.Append("\t")
						Console.Write("\t")
				if key == ConsoleKey.Backspace:
					if Console.CursorLeft > 3 and _buffer.Length > 0:
						Console.CursorLeft--
						Console.Write(" ")
						Console.CursorLeft--
						_buffer.Length--
					#else:	#TODO: flash background?
					#	Console.Beep()
				if CanAutoComplete:
					if key == ConsoleKey.LeftArrow:
						if _selectedSuggestionIndex > 0:
							_selectedSuggestionIndex--
						else:
							_selectedSuggestionIndex = len(_suggestions)					 
						DisplaySuggestions(_buffer.ToString())
					if key == ConsoleKey.RightArrow:
						if _selectedSuggestionIndex+1 < len(_suggestions):
							_selectedSuggestionIndex++
						else:
							_selectedSuggestionIndex = 0
						DisplaySuggestions(_buffer.ToString())
					if newLine:					
						AutoComplete()
						continue
				if not newLine:
					continue
					
			_selectedSuggestionIndex = -1
			
			_buffer.Append(keyChar) if not newLine and not control
			Console.Write(keyChar) if not newLine
			Console.Write(Environment.NewLine) if newLine
			if newLine:
				line = _buffer.ToString()
				
				#TODO: refactor this into runCommand(line)
				if line == "/q":						
					break				
				if line == "/?":
					DisplayHelp()
					_selectedSuggestionIndex = -1
					_buffer.Length = 0
					ConsolePrintPrompt()
					continue
				
				_buffer.Append(Environment.NewLine)
				line = _buffer.ToString()
				
				try:
					_indent++ if lastChar.ToString() in _blockStarters
					_indent-- if lastChar == keyChar and _indent > 0
					if _indent == 0:
						_buffer.Length = 0
						InternalLoopEval(line)
				except x as System.Reflection.TargetInvocationException:
					ConsolePrintException(x.InnerException)
				except x:
					ConsolePrintException(x)
				ConsolePrintPrompt()
			
			lastChar = keyChar	

		DisplayGoodbye()
		
	
	private _indent as int = 0
		
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
		Console.ForegroundColor = _exceptionColor
		for problem as duck in problems:
			markLocation(problem.LexicalInfo)
			type = ("WARNING", "ERROR")[problem isa CompilerError]
			_print("${type}: ${problem.Message}")
		Console.ResetColor()
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


	def DisplayHelp():
		Console.ForegroundColor = _interpreterColor		
		print """Welcome to booish, an interpreter for the boo programming language.
Running boo ${BooVersion} in CLR v${Environment.Version}.

The following builtin functions are available:
    /d[ir] type : lists the members of a type
    /h[elp] type : prints detailed information about a type 
    /l[oad] file : evals an external boo file
    /g[lobals] : returns names of all variables known to interpreter
    /n[ames] : namespace navigation
    /q[uit] : exits the interpreter

Enter boo code in the prompt below."""
		Console.ResetColor()
		

	def DisplayGoodbye():	// booish is friendly
		Console.ForegroundColor = _interpreterColor
		print ""
		print "Have a nice day!" if date.Now.Hour < 16
		print "Have a nice evening!" if date.Now.Hour >= 16
		Console.ResetColor()


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
			
	static def DescribeEntity(entity as IEntity):
		method = entity as ExternalMethod
		if method is not null:
			return InteractiveInterpreter.DescribeMethod(method.MethodInfo)
		field = entity as ExternalField
		if field is not null:
			return InteractiveInterpreter.DescribeField(field.FieldInfo)
		property = entity as ExternalProperty
		if property is not null:
			return InteractiveInterpreter.DescribeProperty(property.PropertyInfo)
		e = entity as ExternalEvent
		if e is not null:
			return InteractiveInterpreter.DescribeEvent(e.EventInfo)
		return entity.ToString()
			
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
		return DescribeModifiers(e.GetAddMethod(true) or e.GetRemoveMethod(true))
		
	static def DescribeModifiers(p as Reflection.PropertyInfo):
		accessor = p.GetGetMethod(true) or p.GetSetMethod(true)
		return DescribeModifiers(accessor)
			
	static def DescribePropertyParameters(parameters as (Reflection.ParameterInfo)):
		return "" if 0 == len(parameters)
		return "(${DescribeParameters(parameters)})"
			
	static def DescribeParameters(parameters as (Reflection.ParameterInfo)):
		return join(DescribeParameter(p) for p in parameters, ", ")
		
	static def DescribeParameter(p as Reflection.ParameterInfo):
		return "${p.Name} as ${GetBooTypeName(p.ParameterType)}"
		
	static def GetBooTypeName(type as System.Type) as string:
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


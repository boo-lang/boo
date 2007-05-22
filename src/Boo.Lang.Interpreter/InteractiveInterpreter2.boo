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
	_blockStarters = (char(':'), char('\\'),)
	
	[getter(LastValue)]
	_lastValue
	
	[property(Print, value is not null)]
	_print as callable(object) = print
	
	def constructor():
		super()
		InitializeStandardReferences()
		LoadHistory()
		
	def constructor(parser as ICompilerStep):
		super(parser)
		InitializeStandardReferences()
		LoadHistory()
		
	def Reset():
		_values.Clear()
		_declarations.Clear()
		_lastValue = null
		_indent = 0
		InitializeStandardReferences()
	

	public final static HISTORY_FILENAME = "booish_history"
	public final static HISTORY_CAPACITY = 100	
	protected _history = System.Collections.Generic.Queue of string(HISTORY_CAPACITY)
	protected _historyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), HISTORY_FILENAME)
	protected _historyIndex = 0
	protected _session = System.Collections.Generic.List of string()
		
	private _buffer = StringBuilder()	#buffer to be executed
	private _line = StringBuilder()		#line being edited

	
	CurrentPrompt as string:
		get:
			if _indent > 0:
				return BlockPrompt
			return DefaultPrompt
	
	
	[property(DefaultPrompt)]
	_defaultPrompt = ">>>"

	[property(BlockPrompt)]
	_blockPrompt = "..."

	[property(InterpreterColor)] # messages from the interpreter (not from user code)
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
	
	CanAutoComplete as bool:
		get:
			return _selectedSuggestionIndex >= 0 and _suggestions is not null
	
	
	private _builtins as (IEntity)
	private _filter as string


	private def ConsolePrintPrompt():
		Console.ForegroundColor = _promptColor
		Console.Write(CurrentPrompt)
		#TODO: automatic indentation option ?
		#for i in range(_indent):
		#	_line.Append("\t")
		#	Console.Write("\t")
		Console.ResetColor()

	private def ConsolePrintMessage(msg as string):
		Console.ForegroundColor = _interpreterColor
		print msg
		Console.ResetColor()

	private def ConsolePrintException(e as Exception):
		Console.ForegroundColor = _exceptionColor
		print e
		Console.ResetColor()

	private def ConsolePrintError(msg as string):
		Console.ForegroundColor = _exceptionColor
		print msg
		Console.ResetColor()

	protected def ConsolePrintSuggestions():
		cursorLeft = Console.CursorLeft
		#cursorTop = Console.CursorTop
		Console.Write(Environment.NewLine)
		
		i = 0

		if _suggestions isa (string):
			for l in _suggestions as (string):
				Console.ForegroundColor = _suggestionsColor
				Console.Write(", ") if i > 0
				if i > 20: #TODO: maxcandidates pref + paging?
					Console.Write("... (too much candidates)")
					break
				if i == _selectedSuggestionIndex:
					Console.ForegroundColor = _selectedSuggestionColor			
				Console.Write(l)
				i++	

		elif _suggestions isa (IEntity):
			for e in _suggestions as (IEntity):
				Console.ForegroundColor = _suggestionsColor
				Console.Write(", ") if i > 0
				if i > 20: #TODO: maxcandidates pref + paging?
					Console.Write("... (too much candidates)")
					break
				if i == _selectedSuggestionIndex:
					Console.ForegroundColor = _selectedSuggestionColor			
				Console.Write(DescribeEntity(e))
				i++
		
		Console.ResetColor()
		#Console.CursorTop = cursorTop
		Console.Write(Environment.NewLine)
		ConsolePrintPrompt()
		Console.Write(_line.ToString())
		Console.CursorLeft = cursorLeft


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
				Console.Write(completion) #FIXME: insert at cursor!
				_line.Append(completion) 
				if not m.GetParameters() or m.GetParameters().Length == 0:
					Console.Write('()') #FIXME: insert at cursor!
					_line.Append('()')
				else:
					Console.Write('(') #FIXME: insert at cursor!
					_line.Append('(')
			
			else: # not a Method
				completion = s.Name[len(_filter):]
				Console.Write(completion) #FIXME: insert at cursor!
				_line.Append(completion)
		else:
			completion = (_suggestions[_selectedSuggestionIndex] as string)[len(_filter):]
			Console.Write(completion) #FIXME: insert at cursor!
			_line.Append(completion)
			
		_selectedSuggestionIndex = -1
		_suggestions = null
	
	
	private _beforeHistory = string.Empty
	
	def DisplayHistory():
		if _history.Count == 0 or _historyIndex < 0 or _historyIndex > _history.Count:
			return
		Console.CursorLeft = len(CurrentPrompt)
		Console.Write(string.Empty.PadLeft(_line.Length, char(' ')))
		line = _history.ToArray()[_historyIndex]
		_line.Length = 0
		_line.Append(line)
		Console.CursorLeft = len(CurrentPrompt)
		Console.Write(line)
		
	
	def ConsoleLoopEval():
		Console.CursorVisible = true
		ConsolePrintPrompt()
		
		lastChar = char('0')
		key = ConsoleKey.LeftArrow
		while key != ConsoleKey.Escape and not _quit:						
			cki = Console.ReadKey(true) #TODO: fix mono different behavior when ()
			key = cki.Key
			keyChar = cki.KeyChar
			control = false
			
			newLine = keyChar in Environment.NewLine

			if char.IsControl(keyChar):
				control = true
				if keyChar == char('\t'):
					test = char(' ')
					test = _line.ToString()[_line.Length-1] if _line.Length > 0
					if char.IsLetterOrDigit(test) or test == char('.'):
						_selectedSuggestionIndex = 0
						DisplaySuggestions(_line.ToString())					
					else:
						_line.Append("    ") #TODO: _tabWidth pref
						Console.Write("    ")
						
				#line-editing support
				if key == ConsoleKey.Backspace:
					if Console.CursorLeft > len(CurrentPrompt) and _line.Length > 0:
						cx = Console.CursorLeft-len(CurrentPrompt)-1
						_line.Remove(cx, 1)
						cx2 = --Console.CursorLeft
						Console.Write("${_line.ToString(cx, _line.Length-cx)} ")
						Console.CursorLeft = cx2
				if key == ConsoleKey.Delete:
					if Console.CursorLeft >= len(CurrentPrompt) and _line.Length > 0:
						cx = Console.CursorLeft-len(CurrentPrompt)
						if cx < _line.Length:
							_line.Remove(cx, 1)
							cx2 = Console.CursorLeft
							Console.Write("${_line.ToString(cx, _line.Length-cx)} ")
							Console.CursorLeft = cx2
				if key == ConsoleKey.LeftArrow:
					if Console.CursorLeft > len(CurrentPrompt) and _line.Length > 0:
						Console.CursorLeft--
				if key == ConsoleKey.RightArrow:
					if Console.CursorLeft < (len(CurrentPrompt)+_line.Length):
						Console.CursorLeft++
				if key == ConsoleKey.Home:
					Console.CursorLeft = len(CurrentPrompt)
				if key == ConsoleKey.End:
					Console.CursorLeft = len(CurrentPrompt) + _line.Length

				#history support
				if key == ConsoleKey.UpArrow:
					if _historyIndex > 0:
						_historyIndex--
						DisplayHistory()
				if key == ConsoleKey.DownArrow:
					if _historyIndex < _history.Count-1:
						_historyIndex++
						DisplayHistory()
					
				#auto-completion support
				if CanAutoComplete:
					if key == ConsoleKey.LeftArrow:
						if _selectedSuggestionIndex > 0:
							_selectedSuggestionIndex--
						else:
							_selectedSuggestionIndex = len(_suggestions)					 
						DisplaySuggestions(_line.ToString())
					if key == ConsoleKey.RightArrow:
						if _selectedSuggestionIndex+1 < len(_suggestions):
							_selectedSuggestionIndex++
						else:
							_selectedSuggestionIndex = 0
						DisplaySuggestions(_line.ToString())
					if newLine:
						AutoComplete()
						continue
				if not newLine:
					continue
			
			_selectedSuggestionIndex = -1
			
			cx = Console.CursorLeft-len(CurrentPrompt)			
			line = _line.ToString()
			
			if not newLine:
				if cx < len(line):	#line-editing support
					_line.Insert(cx, keyChar) if not control
					Console.Write(_line.ToString(cx, _line.Length-cx))
					Console.CursorLeft = len(CurrentPrompt)+cx+1
				else:
					_line.Append(keyChar) if not control
					Console.Write(keyChar)
			
			line = _line.ToString()
			
			if newLine:
				Console.Write(Environment.NewLine)
								
				if not TryRunCommand(line):
					_buffer.Append(line)
					_buffer.Append(Environment.NewLine)
					AddToHistory(line)
					
					try:
						if len(line) > 0:
							_indent++ if line[len(line)-1] in _blockStarters
							_indent-- if line[len(line)-1] == keyChar and _indent > 0
						else:
							_indent--
						if _indent == 0:
							InternalLoopEval(_buffer.ToString())
							_buffer.Length = 0
					except x as System.Reflection.TargetInvocationException:
						ConsolePrintException(x.InnerException)
					except x:
						ConsolePrintException(x)
					
					_line.Length = 0
					ConsolePrintPrompt()
			
			lastChar = keyChar

		SaveHistory()
		DisplayGoodbye()


	/* returns false if no command has been processed, true otherwise */
	def TryRunCommand(line as string):
		if not line.StartsWith("/"):
			return false
		
		cmd = line.Split()
		
		if len(cmd) == 1:
			if cmd[0] == "/q" or cmd[0] == "/quit":						
				quit()
			elif cmd[0] == "/?" or cmd[0] == "/h" or cmd[0] == "/help":
				DisplayHelp()
				_selectedSuggestionIndex = -1
				_buffer.Length = 0
				ConsolePrintPrompt()
			elif cmd[0] == "/g" or cmd[0] == "/globals":
				globals()
			else:
				return false
			
		elif len(cmd) == 2:
			if cmd[0] == "/l" or cmd[0] == "/load":
				load(cmd[1])
				_line.Length = 0
				ConsolePrintPrompt()
			elif cmd[0] == "/s" or cmd[0] == "/save":
				save(cmd[1])
				_line.Length = 0
				ConsolePrintPrompt()
			else:
				return false
		
		else:
			return false
		return true

	
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
			_session.Add(code)
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
    /s[ave] file : writes your current booish session into file
    /g[lobals] : returns names of all variables known to interpreter
    /n[ames] : namespace navigation
    /q[uit] : exits the interpreter (escape key works too)

Enter boo code in the prompt below."""
		Console.ResetColor()
		

	def DisplayGoodbye():	// booish is friendly
		Console.ForegroundColor = _interpreterColor
		print ""
		print "All your boo are belong to us!"
		Console.ResetColor()


	def LoadHistory():
		try:
			using history = File.OpenText(_historyFile):
				while line = history.ReadLine():
					AddToHistory(line)
		except:
			ConsolePrintError("Cannot load history from '${_historyFile}'")

	def AddToHistory(line as string):
		return if 0 == (len(line) - len(Environment.NewLine))
		_history.Dequeue() if _history.Count >= HISTORY_CAPACITY
		line = line.Replace(Environment.NewLine, "")
		_history.Enqueue(line)
		_historyIndex = _history.Count
		
	def SaveHistory():
		try:
			using sw = System.IO.File.CreateText(_historyFile):
				for line in _history:
					sw.WriteLine(line)
		except:
			ConsolePrintError("Cannot save history to '${_historyFile}'.")
		

	def globals():
		return array(string, _values.Keys)		
					
	def dir([required] obj):
		type = (obj as Type) or obj.GetType()
		return array(member for member in type.GetMembers()
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
	
	def save([required] path as string):
		if path is string.Empty:
			path = "booish_session.boo"
		elif not path.EndsWith(".boo"):
			path = "${path}.boo"
		try:
			using sw = System.IO.File.CreateText(path):
				for line in _session:
					sw.Write(line)
			ConsolePrintMessage("Session saved to '${path}'.")
		except:
			ConsolePrintError("Cannot save to '${path}'. Check if path is valid and has correct permissions.")
	
	def help(obj):		
		type = (obj as Type) or obj.GetType()
		DescribeType("    ", type)

	private _quit = false	
	def quit():
		_quit = true

	
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


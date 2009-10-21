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
import System.Collections.Generic
import System.IO
import System.Reflection
import System.Text
import System.Text.RegularExpressions
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

	QQBegin = "[|"
	QQEnd= "|]"

	[getter(LastValue)]
	_lastValue = null

	[property(Print, value is not null)]
	_print as Action[of object] = print

	_entityNameComparer = EntityNameComparer()

	def constructor():
		super()
		Initialize()

	def constructor(parser as ICompilerStep):
		super(parser)
		Initialize()

	def Initialize():
		_disableColors = true if Environment.GetEnvironmentVariable("BOOISH_DISABLE_COLORS") is not null
		if not _disableColors: #make sure setting color does not throw an exception
			try:
				Console.ForegroundColor = ConsoleColor.DarkGray
			except:
				_disableColors = true
		_disableAutocompletion = true if Environment.GetEnvironmentVariable("BOOISH_DISABLE_AUTOCOMPLETION") is not null
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
	private _multiline = false			#is the current line a multi-line?


	Line as string:
		get:
			return _line.ToString()

	LineLastChar as char:
		get:
			return _line.Chars[LineLen-1] if LineLen != 0
			return char('\0')

	LineLen as int:
		get:
			return _line.Length
		set:
			_line.Length = value

	LineIndentLen as int:
		get:
			return IndentChars.Length * _indent

	CurrentPrompt as string:
		get:
			if _indent > 0:
				return BlockPrompt
			return DefaultPrompt


	[property(DefaultPrompt)]
	_defaultPrompt = ">>>"

	[property(BlockPrompt)]
	_blockPrompt = "..."

	[property(IndentChars)]
	_indentChars = "    "

	[property(DisableColors)]
	_disableColors = false

	[property(DisableAutocompletion)]
	_disableAutocompletion = false

	[property(InterpreterColor)] # messages from the interpreter (not from user code)
	_interpreterColor = ConsoleColor.DarkGray

	[property(PromptColor)]
	_promptColor = ConsoleColor.DarkGreen

	[property(ExceptionColor)]
	_exceptionColor = ConsoleColor.DarkRed

	[property(SuggestionsColor)]
	_suggestionsColor = ConsoleColor.DarkYellow

	[property(SelectedSuggestionColor)]
	_selectedSuggestionColor = ConsoleColor.DarkMagenta

	[property(SelectedSuggestionIndex)]
	_selectedSuggIdx as int?

	[property(Suggestions)]
	_suggestions as (object)

	CanAutoComplete as bool:
		get:
			return _selectedSuggIdx is not null


	private _builtins as (IEntity)
	private _filter as string


	private def ConsolePrintPrompt():
		ConsolePrintPrompt(true)

	private def ConsolePrintPrompt(autoIndent as bool):
		return if _quit
		Console.ForegroundColor = _promptColor if not _disableColors
		Console.Write(CurrentPrompt)
		Console.ResetColor() if not _disableColors
		if autoIndent and CurrentPrompt == BlockPrompt:
			for i in range(_indent):
				WriteIndent()

	private def ConsolePrintMessage(msg as string):
		Console.ForegroundColor = _interpreterColor if not _disableColors
		print msg
		Console.ResetColor() if not _disableColors

	private def ConsolePrintException(e as Exception):
		Console.ForegroundColor = _exceptionColor if not _disableColors
		print e
		Console.ResetColor() if not _disableColors

	private def ConsolePrintError(msg as string):
		Console.ForegroundColor = _exceptionColor if not _disableColors
		print msg
		Console.ResetColor() if not _disableColors

	protected def ConsolePrintSuggestions():
		cursorLeft = Console.CursorLeft
		#cursorTop = Console.CursorTop
		Console.Write(Environment.NewLine)

		i = 0

		Array.Sort(_suggestions) if _suggestions isa (string)
		Array.Sort[of IEntity](_suggestions, _entityNameComparer) if _suggestions isa (IEntity)

		for s in _suggestions as (object):
			Console.ForegroundColor = _suggestionsColor if not _disableColors
			Console.Write(", ") if i > 0
			if i > 20: #TODO: maxcandidates pref + paging?
				Console.Write("... (too many candidates)")
				break
			if i == _selectedSuggIdx:
				Console.ForegroundColor = _selectedSuggestionColor if not _disableColors
			if s isa IEntity:
				Console.Write(Help.DescribeEntity(s as IEntity))
			else:
				Console.Write(s)
			i++

		Console.ResetColor() if not _disableColors
		#Console.CursorTop = cursorTop
		Console.Write(Environment.NewLine)
		ConsolePrintPrompt(false)
		Console.Write(Line)
		Console.CursorLeft = cursorLeft

	protected def Write(s as string):
		Console.Write(s)
		_line.Append(s)

	protected def WriteIndent():
		Write(IndentChars)

	protected def Indent():
		WriteIndent()
		_indent++

	protected def Unindent():
		return if _indent == 0
		Delete(IndentChars.Length)
		_indent--

	protected def Delete(count as int): #if count is 0, forward-delete
		cx = Console.CursorLeft-len(CurrentPrompt)-count
		return if cx < LineLen and count == 0
		dcount = (count if count != 0 else 1)
		_line.Remove(cx, dcount)
		curX = Console.CursorLeft - dcount
		Console.CursorLeft = curX
		Console.Write("${_line.ToString(cx, LineLen-cx)} ")
		Console.CursorLeft = curX


	private static re_open = Regex("\\(", RegexOptions.Singleline)
	private static re_close = Regex("\\)", RegexOptions.Singleline)

	def DisplaySuggestions():
		DisplaySuggestions(Line)

	def DisplaySuggestions(query as string):
		return if DisableAutocompletion

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
			_selectedSuggIdx = null
			#Console.Beep() #TODO: flash background?
		elif 1 == len(_suggestions):
			AutoComplete()
		else:
			ConsolePrintSuggestions()


	def AutoComplete():
		raise InvalidOperationException("no suggestions") if _suggestions is null or _selectedSuggIdx is null

		s = _suggestions[_selectedSuggIdx.Value] as IEntity
		completion = (s.Name[len(_filter):] if s is not null else (_suggestions[_selectedSuggIdx.Value] as string)[len(_filter):])
		Write(completion)
		if s and s.EntityType == EntityType.Method:
			Write("(")
			m = s as IMethod
			Write(')') if m.GetParameters().Length == 0

		_selectedSuggIdx = null
		_suggestions = null


	private _beforeHistory = string.Empty

	def DisplayHistory():
		if _history.Count == 0 or _historyIndex < 0 or _historyIndex > _history.Count:
			return
		Console.CursorLeft = len(CurrentPrompt)
		Console.Write(string.Empty.PadLeft(LineLen, char(' ')))
		line = _history.ToArray()[_historyIndex]
		LineLen = 0
		Console.CursorLeft = len(CurrentPrompt)
		Write(line)


	def ConsoleLoopEval():
		Console.CursorVisible = true
		ConsolePrintPrompt()

		lastChar = char('0')
		key = ConsoleKey.LeftArrow
		while not _quit:						
			cki = Console.ReadKey(true)
			key = cki.Key
			keyChar = cki.KeyChar
			control = false

			newLine = keyChar in Environment.NewLine

			if char.IsControl(keyChar):
				control = true
				if keyChar == char('\t'):
					if LineLen > 0 and (char.IsLetterOrDigit(LineLastChar) or LineLastChar == char('.')):
						_selectedSuggIdx = 0
						DisplaySuggestions()
					else:
						Indent()

				#line-editing support
				if not _multiline and LineLen > 0:
					if Console.CursorLeft > len(CurrentPrompt):
						if key == ConsoleKey.Backspace:
							if _indent > 0 and LineLen == LineIndentLen:
								Unindent()
							else:
								Delete(1)
						elif key == ConsoleKey.LeftArrow:
							Console.CursorLeft--
					if key == ConsoleKey.Delete:
						Delete(0)
					elif key == ConsoleKey.RightArrow:
						if Console.CursorLeft < (len(CurrentPrompt)+LineLen):
							Console.CursorLeft++
					elif key == ConsoleKey.Home:
						Console.CursorLeft = len(CurrentPrompt)
					elif key == ConsoleKey.End:
						Console.CursorLeft = len(CurrentPrompt) + LineLen

				#history support
				if key == ConsoleKey.UpArrow:
					if _historyIndex > 0:
						_historyIndex--
						DisplayHistory()
				elif key == ConsoleKey.DownArrow:
					if _historyIndex < _history.Count-1:
						_historyIndex++
						DisplayHistory()

				#auto-completion support
				if CanAutoComplete:
					if key == ConsoleKey.LeftArrow:
						if _selectedSuggIdx > 0:
							_selectedSuggIdx--
						else:
							_selectedSuggIdx = len(_suggestions) - 1
						DisplaySuggestions()
					elif key == ConsoleKey.RightArrow:
						if _selectedSuggIdx < len(_suggestions) - 1:
							_selectedSuggIdx++
						else:
							_selectedSuggIdx = 0
						DisplaySuggestions()
					if newLine:
						AutoComplete()
						continue
				if not newLine:
					continue

			_selectedSuggIdx = null

			cx = Console.CursorLeft-len(CurrentPrompt)
			#multi-line?
			if cx < 0 or LineLen >= Console.WindowWidth-len(CurrentPrompt):
				cx = LineLen
				_multiline = true

			if not newLine:
				#line-editing support
				if cx < LineLen and not _multiline:
					_line.Insert(cx, keyChar) if not control
					Console.Write(_line.ToString(cx, LineLen-cx))
					Console.CursorLeft = len(CurrentPrompt)+cx+1
				else:
					_line.Append(keyChar) if not control
					Console.Write(keyChar)

			if newLine:
				_multiline = false
				Console.Write(Environment.NewLine)

				if not TryRunCommand(Line):
					_buffer.Append(Line)
					_buffer.Append(Environment.NewLine)
					AddToHistory(Line)

					_indent++ if LineLastChar in _blockStarters or Line.EndsWith(QQBegin)
					_indent-- if Line.EndsWith(IndentChars+"pass")
					if Line.EndsWith(QQEnd):
						CheckBooLangCompilerReferenced()
						_indent--

					if _indent <= 0:
						_indent = 0
						try:
							InternalLoopEval(_buffer.ToString())
						except x as System.Reflection.TargetInvocationException:
							ConsolePrintException(x.InnerException)
						except x:
							ConsolePrintException(x)
						ensure:
							_buffer.Length = 0 #truncate buffer

				LineLen = 0 #truncate line
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
				ConsolePrintPrompt()
			elif cmd[0] == "/g" or cmd[0] == "/globals":
				InternalLoopEval("globals()")
				ConsolePrintPrompt()
			else:
				return false

		elif len(cmd) == 2:
			if cmd[0] == "/l" or cmd[0] == "/load":
				load(cmd[1])
				ConsolePrintPrompt()
			elif cmd[0] == "/s" or cmd[0] == "/save":
				save(cmd[1])
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
		Console.ForegroundColor = _exceptionColor if not _disableColors
		for problem as duck in problems:
			markLocation(problem.LexicalInfo)
			type = ("WARNING", "ERROR")[problem isa CompilerError]
			_print("${type}: ${problem.Message}")
		Console.ResetColor() if not _disableColors
		if problems.Count > 0:
			return true
		return false

	private def markLocation(location as LexicalInfo):
		pos = location.Column
		_print("---" + "-" * pos + "^") if pos > 0

	private def InitializeStandardReferences():
		SetValue("interpreter", self)
		SetValue("dir", dir)
		SetValue("describe", describe)
		SetValue("print", { value | _print(value) })
		SetValue("load", load)
		SetValue("globals", globals)
		SetValue("quit", quit)
		SetValue("getRootNamespace", Namespace.GetRootNamespace)


	def DisplayLogo():
		Console.ForegroundColor = _interpreterColor	if not _disableColors
		print """Welcome to booish, an interactive interpreter for the boo programming language.
Running boo ${BooVersion} on ${Boo.Lang.Runtime.RuntimeServices.RuntimeDisplayName}.

Enter boo code in the prompt below (or type /help)."""
		Console.ResetColor() if not _disableColors

	def DisplayHelp():
		Console.ForegroundColor = _interpreterColor	if not _disableColors
		print """The following builtin functions are available :
    dir(type) : returns the members of a type
    describe(type) : describe a type as boo code
    globals() or /g : returns names of all variables known to interpreter
    load(file) or /l file : evals an external boo file
    save(file) or /s file : writes your current booish session into file
    quit() or /q : exits the interpreter"""
		Console.ResetColor() if not _disableColors


	def DisplayGoodbye():	// booish is friendly
		Console.ForegroundColor = _interpreterColor if not _disableColors
		print ""
		print "All your boo are belong to us!"
		Console.ResetColor() if not _disableColors


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
			ConsolePrintMessage("Evaluating '${path}' ...")
			result = EvalCompilerInput(FileInput(path))
			if ShowWarnings:
				DisplayProblems(result.Warnings)
			if not DisplayProblems(result.Errors):
				ProcessLastValue()
		else:
			try:
				ConsolePrintMessage("Adding reference to '${path}'")
				References.Add(System.Reflection.Assembly.LoadFrom(path))
			except e:
				ConsolePrintError(e.Message)

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

	def describe(obj):
		type = (obj as Type) or obj.GetType()
		for line in Help.HelpFormatter("    ").GenerateFormattedLinesFor(type):
			_print(line)

	private _quit = false	
	def quit():
		_quit = true
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


	private def CheckBooLangCompilerReferenced():
		return if _blcReferenced
		blcAssembly = typeof(Boo.Lang.Compiler.CompilerContext).Assembly
		referenced = false
		for refr in _compiler.Parameters.References:
			referenced |= (refr == blcAssembly)
		if not referenced:
			_compiler.Parameters.AddAssembly(blcAssembly)
		_blcReferenced = true

	_blcReferenced = false


class EntityNameComparer(IComparer of IEntity):
	def Compare(a as IEntity, b as IEntity) as int:
		return string.Compare(a.Name, b.Name)


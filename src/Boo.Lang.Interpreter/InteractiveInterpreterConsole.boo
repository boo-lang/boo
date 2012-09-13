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
import System.Linq
import System.Text
import System.Text.RegularExpressions
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import PatternMatching
import Environments

class InteractiveInterpreterConsole:
	
	public final static HISTORY_FILENAME = "booish_history"
	public final static HISTORY_CAPACITY = 100	
	
	_history = System.Collections.Generic.Queue of string(HISTORY_CAPACITY)
	_historyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), HISTORY_FILENAME)
	_historyIndex = 0
	_session = System.Collections.Generic.List of string()

	_buffer = StringBuilder()	#buffer to be executed
	_line = StringBuilder()		#line being edited
	_multiline = false			#is the current line a multi-line?

	[property(BlockStarters, value is not null)]
	_blockStarters = (char(':'), char('\\'),)
	
	[property(ShowWarnings)]
	_showWarnings = false

	QQBegin = "[|"
	QQEnd= "|]"
	
	_interpreter as InteractiveInterpreter
	
	def constructor():
		self(InteractiveInterpreter())

	def constructor(interpreter as InteractiveInterpreter):
		_interpreter = interpreter
		_interpreter.RememberLastValue = true
		
		DisableColors = not string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BOOISH_DISABLE_COLORS"))
		if not DisableColors: #make sure setting color does not throw an exception
			try:
				Console.ForegroundColor = ConsoleColor.DarkGray
			except:
				DisableColors = true
		DisableAutocompletion = not string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BOOISH_DISABLE_AUTOCOMPLETION"))
		
		_interpreter.SetValue("load", Load)
		_interpreter.SetValue("save", Save)
		_interpreter.SetValue("quit", Quit)

		LoadHistory()
		
	def SetValue(name as string, value):
		_interpreter.SetValue(name, value)
	
	Line:
		get: return _line.ToString()

	LineLastChar:
		get: return (_line.Chars[LineLen-1] if LineLen != 0 else char('\0'))

	LineLen:
		get: return _line.Length
		set: _line.Length = value

	LineIndentLen:
		get: return IndentChars.Length * _indent

	CurrentPrompt as string:
		get: return (BlockPrompt if _indent > 0 else DefaultPrompt)
		
	PrintModules:
		get: return _interpreter.Pipeline.Find(Boo.Lang.Compiler.Steps.PrintBoo) != -1
		set:
			if value:
				if not PrintModules:
					_interpreter.Pipeline.Add(Boo.Lang.Compiler.Steps.PrintBoo())
			else:
				_interpreter.Pipeline.Remove(Boo.Lang.Compiler.Steps.PrintBoo)
				

	property DefaultPrompt = ">>> "

	property BlockPrompt = "... "

	property IndentChars = "    "

	property DisableColors = false

	property DisableAutocompletion = false

	# messages from the interpreter (not from user code)
	property InterpreterColor = ConsoleColor.DarkGray 

	property PromptColor = ConsoleColor.DarkGreen

	property ExceptionColor = ConsoleColor.DarkRed
	
	property WarningColor = ConsoleColor.Yellow
	
	property ErrorColor = ConsoleColor.Red

	property SuggestionsColor = ConsoleColor.DarkYellow

	property SelectedSuggestionColor = ConsoleColor.DarkMagenta

	_selectedSuggestionIndex as int?

	_suggestions as EnvironmentBoundValue[of (object)]

	CanAutoComplete as bool:
		get: return _selectedSuggestionIndex is not null

	private _builtins as (IEntity)
	private _filter as string
	
	def Eval(code as string):
		try:
			result = _interpreter.Eval(code)
			DisplayResults(result)
			return result
		except x as System.Reflection.TargetInvocationException:
			ConsolePrintException(x.InnerException)
		except x:
			ConsolePrintException(x)
		
	private def DisplayResults(results as CompilerContext):
		if ShowWarnings:
			DisplayProblems(results.Warnings, WarningColor)
		if not DisplayProblems(results.Errors, ErrorColor):
			ProcessLastValue()

	private def ConsolePrintPrompt():
		ConsolePrintPrompt(true)

	private def ConsolePrintPrompt(autoIndent as bool):
		return if _quit
		WithColor PromptColor:
			Console.Write(CurrentPrompt)
		if autoIndent and CurrentPrompt == BlockPrompt:
			for i in range(_indent):
				WriteIndent()
				
	private def WithColor(color as ConsoleColor, block as System.Action):
		if DisableColors:
			block()
		else:
			Console.ForegroundColor = color
			try:
				block()
			ensure:
				Console.ResetColor()

	private def ConsolePrintMessage(msg as string):
		WithColor InterpreterColor:
			print msg

	private def ConsolePrintException(e as Exception):
		WithColor ExceptionColor:
			print e

	private def ConsolePrintError(msg as string):
		WithColor ExceptionColor:
			print msg
			
	private def NewLine():
		Console.Write(Environment.NewLine)

	protected def ConsolePrintSuggestions():
		cursorLeft = Console.CursorLeft
		#cursorTop = Console.CursorTop
		NewLine()

		i = 0

		for s in SuggestionDescriptions():
			Console.ForegroundColor = SuggestionsColor if not DisableColors
			Console.Write(", ") if i > 0
			if i > 20: #TODO: maxcandidates pref + paging?
				Console.Write("... (too many candidates)")
				break
			if i == _selectedSuggestionIndex:
				Console.ForegroundColor = SelectedSuggestionColor if not DisableColors
			Console.Write(s)
			i++

		Console.ResetColor() if not DisableColors
		#Console.CursorTop = cursorTop
		NewLine()
		ConsolePrintPrompt(false)
		Console.Write(Line)
		Console.CursorLeft = cursorLeft
		
	def SuggestionDescriptions():
		return _suggestions.Select(DescriptionsFor).Value
		
	def DescriptionsFor(suggestions as (object)):
		return array((DescriptionFor(s) for s in suggestions).Distinct())
		
	def DescriptionFor(s):
		match s:
			case e = IEntity():
				return Boo.Lang.Interpreter.Builtins.DescribeEntity(e)
			otherwise:
				return s.ToString()

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
			_filter = query[query.LastIndexOf('.')+1:]
			_suggestions = (
				_interpreter
				.SuggestCompletionsFor(codeToComplete+"__codecomplete__")
				.Select[of (object)](
					{ es | array(e as object for e in es if e.Name.StartsWith(_filter)) }))			

		if _suggestions.Value is null or 0 == len(_suggestions.Value): #suggest a  var		
			_filter = query
			_suggestions = EnvironmentBoundValue.Create(
							null,
							array(
								var.Key.ToString() as object
								for var in _interpreter.Values
								if var.ToString().StartsWith(_filter)))

		if _suggestions.Value is null or 0 == len(_suggestions.Value):
			_selectedSuggestionIndex = null
			#Console.Beep() #TODO: flash background?
		elif 1 == len(_suggestions.Value):
			AutoComplete()
		else:
			ConsolePrintSuggestions()


	def AutoComplete():
		raise InvalidOperationException("no suggestions") if _suggestions.Value is null or _selectedSuggestionIndex is null

		Write(_suggestions.Select[of string]({ ss | AutoCompletionFor(ss[_selectedSuggestionIndex.Value]) }).Value)
		
		_selectedSuggestionIndex = null
		_suggestions = EnvironmentBoundValue[of (object)](null, null)
		
	def AutoCompletionFor(s):
		match s:
			case m = IMethod(Name: name):
				if len(m.GetParameters()) == 0:
					return "$(TrimFilter(name))()"
				return "$(TrimFilter(name))("
			case IEntity(Name: name):
				return TrimFilter(name)
			otherwise:
				return TrimFilter(s.ToString())
				
	def TrimFilter(s as string):
		return s[len(_filter):]

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

	def ReadEvalPrintLoop():
		Console.CursorVisible = true
		ConsolePrintPrompt()
		while not _quit:
			ReadEvalPrintLoopStep()
		SaveHistory()
		DisplayGoodbye()
		
	private def ReadEvalPrintLoopStep():
		cki = Console.ReadKey(true)
		key = cki.Key
		keyChar = cki.KeyChar
		control = false

		newLine = keyChar in Environment.NewLine

		if char.IsControl(keyChar):
			control = true
			if keyChar == char('\t'):
				if LineLen > 0 and (char.IsLetterOrDigit(LineLastChar) or LineLastChar == char('.')):
					_selectedSuggestionIndex = 0
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
					if _selectedSuggestionIndex > 0:
						_selectedSuggestionIndex--
					else:
						_selectedSuggestionIndex = len(_suggestions.Value) - 1
					DisplaySuggestions()
				elif key == ConsoleKey.RightArrow:
					if _selectedSuggestionIndex < len(_suggestions.Value) - 1:
						_selectedSuggestionIndex++
					else:
						_selectedSuggestionIndex = 0
					DisplaySuggestions()
				if newLine:
					AutoComplete()
					return
			if not newLine:
				return

		_selectedSuggestionIndex = null

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
						Eval(_buffer.ToString())
					ensure:
						_buffer.Length = 0 #truncate buffer

			LineLen = 0 #truncate line
			ConsolePrintPrompt()

	/* returns false if no command has been processed, true otherwise */
	def TryRunCommand(line as string):
		if not line.StartsWith("/"):
			return false

		cmd = line.Split()

		if len(cmd) == 1:
			if cmd[0] == "/q" or cmd[0] == "/quit":						
				Quit()
			elif cmd[0] == "/?" or cmd[0] == "/h" or cmd[0] == "/help":
				DisplayHelp()
			elif cmd[0] == "/g" or cmd[0] == "/globals":
				Eval("globals()")
			else:
				return false

		elif len(cmd) == 2:
			if cmd[0] == "/l" or cmd[0] == "/load":
				Load(cmd[1])
			elif cmd[0] == "/s" or cmd[0] == "/save":
				Save(cmd[1])
			else:
				return false

		else:
			return false
		return true


	private _indent as int = 0

	def DisplayLogo():
		WithColor InterpreterColor:
			print """Welcome to booish, an interactive interpreter for the boo programming language.
Running boo ${BooVersion} on ${Boo.Lang.Runtime.RuntimeServices.RuntimeDisplayName}.

Enter boo code in the prompt below (or type /help)."""

	def DisplayHelp():
		WithColor InterpreterColor:
			print """The following builtin functions are available :
    /? or /h or /help : display this help
    dir(type) : returns the members of a type
    describe(type) : describe a type as boo code
    globals() or /g : returns names of all variables known to interpreter
    load(file) or /l file : evals an external boo file
    save(file) or /s file : writes your current booish session into file
    quit() or /q : exits the interpreter"""

	def DisplayGoodbye():	// booish is friendly
		WithColor InterpreterColor:
			print ""
			print "All your boo are belong to us!"

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
			
	def Load([required] path as string):
		if path.EndsWith(".boo"):
			ConsolePrintMessage("Evaluating '${path}' ...")
			DisplayResults(_interpreter.EvalCompilerInput(Boo.Lang.Compiler.IO.FileInput(path)))
		else:
			ConsolePrintMessage("Adding reference to '${path}'")
			try:
				_interpreter.References.Add(System.Reflection.Assembly.LoadFrom(path))
			except e:				
				ConsolePrintException(e)
				
	private def ProcessLastValue():
		_ = _interpreter.LastValue
		if _ is not null:
			Console.WriteLine(Boo.Lang.Interpreter.Builtins.repr(_))
			_interpreter.SetValue("_", _)

	def Save([required] path as string):
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

	private _quit = false
	
	def Quit():
		_quit = true
		
	def DisplayProblems(problems as ICollection, color as ConsoleColor):
		return if problems is null or problems.Count == 0
		WithColor color:
			for problem as duck in problems:
				markLocation(problem.LexicalInfo)
				type = ("ERROR" if problem isa CompilerError else "WARNING")
				Console.WriteLine("${type}: ${problem.Message}")
		return true

	private def markLocation(location as Ast.LexicalInfo):
		pos = location.Column
		Console.WriteLine("--" + "-" * pos + "^") if pos > 0
		
	private def CheckBooLangCompilerReferenced():
		return if _blcReferenced
		_interpreter.References.Add(typeof(Boo.Lang.Compiler.CompilerContext).Assembly)
		_blcReferenced = true
		
	_blcReferenced = false
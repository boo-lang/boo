"""
BOO-736-1.boo(15,42): No appropriate version of 'CompilerGeneratedExtensions.BeginInvoke' for the argument list '(callable(string) as int)' was found.
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

preservingLexicalInfo:
	code = [|
		class Foo:
			def Bar1(parameter as string):
				return 42
	
			def Bar2():
				result = Bar1.BeginInvoke()
	|]

for error in compile_(code).Errors:
	location = error.LexicalInfo
	file = System.IO.Path.GetFileName(location.FileName)
	line = location.Line
	column = location.Column
	message = error.Message
	print "$file($line,$column): $message"

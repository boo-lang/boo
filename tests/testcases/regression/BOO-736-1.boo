"""
BOO-736-1.boo(15,42): The best overload for the method 'callable(string) as int.BeginInvoke(string, System.AsyncCallback, object)' is not compatible with the argument list '()'.
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

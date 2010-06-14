"""
lexical-info-is-preserved.boo
8
"""
import Boo.Lang.Compiler.Ast

preservingLexicalInfo:
	location = [| foo |].LexicalInfo
	print System.IO.Path.GetFileName(location.FileName)
	print location.Line
	
assert LexicalInfo.Empty is [| foo |].LexicalInfo
	

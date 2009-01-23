"""
bonjour
quoi de neuf?
a bientot!
"""

import Boo.Lang.Compiler

macro speak:
	for statement in speak.Body.Statements:
		s = cast(Ast.ExpressionStatement, statement).Expression as Ast.StringLiteralExpression
		if s.Value == "hello":
			yield [| print "bonjour" |]
		if s.Value == "whats up?":
			yield [| print "quoi de neuf?" |]
	yield [| print "a bientot!" |]

speak "french":
	"hello"
	"whats up?"


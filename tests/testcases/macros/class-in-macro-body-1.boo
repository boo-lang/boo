"""
found in one
none in two
"""
namespace ClassInMacroBody

import Boo.Lang.Compiler.Ast

macro scanned(name as ReferenceExpression):
	private class LocalScanner(FastDepthFirstVisitor):
		[Getter(Found)]
		_found as bool

		override def OnReferenceExpression(node as ReferenceExpression):
			_found = true if node.Name == 'marker'

	ls = LocalScanner()
	scanned.Body.Accept(ls)
	if ls.Found:
		yield [|print "found in " + $(name.Name)|]
	else:
		yield [|print "none in " + $(name.Name)|]

scanned one:
	marker = 1

scanned two:
	other = 2

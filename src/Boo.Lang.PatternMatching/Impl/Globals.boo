namespace Boo.Lang.PatternMatching.Impl

import Boo.Lang.Compiler.Ast

def caseListFor(node as MacroStatement) as List:
	list as List = node["caseList"]
	if list is null:
		node["caseList"] = list = []
	return list

def parentMatch(node as MacroStatement):
	match = node.ParentNode.ParentNode as MacroStatement
	assert match.Name == "match"
	return match

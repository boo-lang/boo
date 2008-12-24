namespace Boo.Lang.PatternMatching

import Boo.Lang.Compiler.Ast
	
def caseList(node as MacroStatement) as List:
	list as List = node["caseList"]
	if list is null:
		node["caseList"] = list = []
	return list

def parentMatch(node as MacroStatement):
	match = node.ParentNode.ParentNode as MacroStatement
	assert match.Name == "match"
	return match
	
macro case:
	match = parentMatch(case)
	caseList(match).Add(case)
	
macro otherwise:		
	match = parentMatch(otherwise)
	assert match["otherwise"] is null
	match["otherwise"] = otherwise

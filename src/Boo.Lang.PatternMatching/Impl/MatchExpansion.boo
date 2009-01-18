namespace Boo.Lang.PatternMatching.Impl

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

internal class MatchExpansion:
	
	node as MacroStatement
	expression as Expression
	context as CompilerContext
	public final value as Statement
	
	def constructor(context as CompilerContext, node as MacroStatement):
		self.context = context
		self.node = node
		self.expression = node.Arguments[0]
		self.value = expand(newTemp(expression))
		
	def expand(matchValue as Expression):
		
		topLevel = expanded = expandCase(matchValue, caseListFor(node)[0])
		for case in caseListFor(node)[1:]:
			caseExpansion = expandCase(matchValue, case)
			expanded.FalseBlock = caseExpansion.ToBlock()
			expanded = caseExpansion		
		expanded.FalseBlock = expandOtherwise(matchValue)
		
		return [|
			block:
				$matchValue = $expression
				$topLevel
		|].Block
		
	def expandOtherwise(matchValue as Expression):
		otherwise as MacroStatement = node["otherwise"]
		if otherwise is null: return defaultOtherwise(matchValue)
		return expandOtherwise(otherwise)
		
	def expandOtherwise(node as MacroStatement):
		assert 0 == len(node.Arguments)
		return node.Block
		
	def defaultOtherwise(matchValue as Expression):
		matchError = [| raise MatchError("'" + $(expression.ToCodeString()) + "' failed to match '" + $matchValue + "'") |]
		matchError.LexicalInfo = node.LexicalInfo
		return matchError.ToBlock()
		
	def expandCase(matchValue as Expression, node as MacroStatement):
		assert 1 == len(node.Arguments)
		pattern = node.Arguments[0]
		condition = expandPattern(matchValue, pattern)
		return [| 
			if $condition:
				$(node.Block)
		|]
		
	def expandPattern(matchValue as Expression, pattern as Expression) as Expression:
		return PatternExpander().expand(matchValue, pattern)
		
	

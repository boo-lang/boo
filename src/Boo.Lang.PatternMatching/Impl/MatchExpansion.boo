namespace Boo.Lang.PatternMatching.Impl

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

internal class MatchExpansion:
	
	node as MacroStatement
	expression as Expression
	context as CompilerContext
	[getter(Value)] final value as Statement
	
	def constructor(context as CompilerContext, node as MacroStatement):
		self.context = context
		self.node = node
		self.expression = node.Arguments[0]
		self.value = Expand(NewTemp(expression))
		
	def Expand(matchValue as Expression):
		
		topLevel = expanded = ExpandCase(matchValue, caseListFor(node)[0])
		for case in caseListFor(node)[1:]:
			caseExpansion = ExpandCase(matchValue, case)
			expanded.FalseBlock = caseExpansion.ToBlock()
			expanded = caseExpansion		
		expanded.FalseBlock = ExpandOtherwise(matchValue)
		
		return [|
			block:
				$matchValue = $expression
				$topLevel
		|].Body
		
	def ExpandOtherwise(matchValue as Expression):
		otherwise as MacroStatement = node["otherwise"]
		if otherwise is null: return DefaultOtherwise(matchValue)
		return ExpandOtherwise(otherwise)
		
	def ExpandOtherwise(node as MacroStatement):
		assert 0 == len(node.Arguments)
		return node.Body
		
	def DefaultOtherwise(matchValue as Expression):
		matchError = [| raise MatchError("'" + $(expression.ToCodeString()) + "' failed to match '" + $matchValue + "'") |]
		matchError.LexicalInfo = node.LexicalInfo
		return matchError.ToBlock()
		
	def ExpandCase(matchValue as Expression, node as MacroStatement):
		assert 1 == len(node.Arguments)
		pattern = node.Arguments[0]
		condition = ExpandPattern(matchValue, pattern)
		return [| 
			if $condition:
				$(node.Body)
		|]
		
	def ExpandPattern(matchValue as Expression, pattern as Expression) as Expression:
		return PatternExpander().Expand(matchValue, pattern)

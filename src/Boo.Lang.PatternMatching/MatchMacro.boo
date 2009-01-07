namespace Boo.Lang.PatternMatching

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

# TODO: check for unreacheable patterns
class MatchMacro(AbstractAstMacro):
"""
Pattern matching facility:

	match <expression>:
		case <Pattern1>:
			<block1>
			.
			.
			.
		case <PatternN>:
			<blockN>
		otherwise:
			<blockOtherwise>

The following patterns are supported:
    
    Type() -- type test pattern
    Type(Property1: Pattern1, ...) -- object pattern
    Pattern1 | Pattern2 -- either pattern
    Pattern1 and condition -- constrained pattern  (NOT IMPLEMENTED)
    Pattern1 or condition -- constrained pattern  (NOT IMPLEMENTED)
    (Pattern1, Pattern2) -- fixed size iteration pattern
    [Pattern1, Pattern2] -- arbitrary size iteration pattern (NOT IMPLEMENTED)
    x = Pattern -- variable binding
    x -- variable binding
    BinaryOperatorType.Assign -- constant pattern
    42 -- constant pattern
    "42" -- constant pattern
    null -- null test pattern
    
If no pattern matches MatchError is raised.
"""
	override def Expand(node as MacroStatement):
		assert 0 == len(node.Block.Statements)
		return MatchExpansion(Context, node).value

class MatchExpansion:
	
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
		
		topLevel = expanded = expandCase(matchValue, caseList(node)[0])
		for case in caseList(node)[1:]:
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
		
	

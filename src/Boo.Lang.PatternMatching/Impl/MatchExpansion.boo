#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
		errMsg = "`${expression.ToCodeString()}` ({0}) failed to match"
		matchError = [| raise MatchError(string.Format($errMsg, $matchValue)) |]
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


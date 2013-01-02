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

import Boo.Lang.Compiler.Ast

internal class MatchExpansion:
	
	final _match as MacroStatement
	final _matchValues as (Expression)
	public final Value as Statement
	
	def constructor(match as MacroStatement):
		_match = match
		_matchValues = array(NewTemp(e) for e in _match.Arguments)
		self.Value = Expand()
		
	def Expand():
		topLevel = expanded = ExpandCase(caseListFor(_match)[0])
		for case in caseListFor(_match)[1:]:
			caseExpansion = ExpandCase(case)
			expanded.FalseBlock = caseExpansion.ToBlock()
			expanded = caseExpansion		
		expanded.FalseBlock = ExpandOtherwise()
		
		result = Block()
		for matchValue as Expression, expression as Expression in zip(_matchValues, _match.Arguments):
			result.Add([| $matchValue = $expression |])
		result.Add(topLevel)
		return result
		
	def ExpandOtherwise():
		otherwise as MacroStatement = _match["otherwise"]
		if otherwise is null: return DefaultOtherwise()
		return ExpandOtherwise(otherwise)
		
	def ExpandOtherwise(otherwise as MacroStatement):
		assert 0 == len(otherwise.Arguments)
		return otherwise.Body
		
	def DefaultOtherwise():
		errorMessage = ExpressionInterpolationExpression(_match.Arguments[0].LexicalInfo)
		for matchValue as Expression, expression as Expression in zip(_matchValues, _match.Arguments):
			if len(errorMessage.Expressions) > 0:
				errorMessage.Expressions.Add(StringLiteralExpression.Lift(" or "))
			errorMessage.Expressions.Add(StringLiteralExpression.Lift("`$(expression.ToCodeString())` failed to match `"))
			errorMessage.Expressions.Add(matchValue.CloneNode())
			errorMessage.Expressions.Add(StringLiteralExpression.Lift("`"))
		matchError = [| raise MatchError($errorMessage) |]
		matchError.LexicalInfo = _match.LexicalInfo
		return matchError.ToBlock()
		
	def ExpandCase(case as MacroStatement):
		assert len(_match.Arguments) == len(case.Arguments)
		
		condition as Expression
		for matchValue, pattern in zip(_matchValues, case.Arguments):
			expansion = ExpandPattern(matchValue, pattern)
			if condition is null:
				condition = expansion
			else:
				condition = [| $condition and $expansion |]
			
		return [| 
			if $condition:
				$(case.Body)
		|]
		
	def ExpandPattern(matchValue as Expression, pattern as Expression) as Expression:
		return PatternExpander().Expand(matchValue, pattern)


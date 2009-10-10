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

class PatternExpander:

	private static final Dummy = '_'

	def Expand(matchValue as Expression, pattern as Expression) as Expression:
		if pattern isa MethodInvocationExpression:
			return ExpandObjectPattern(matchValue, pattern)
				
		if pattern isa MemberReferenceExpression:
			return ExpandValuePattern(matchValue, pattern)
				
		if pattern isa ReferenceExpression:
			return ExpandBindPattern(matchValue, pattern)
				
		if pattern isa QuasiquoteExpression:
			return ExpandQuasiquotePattern(matchValue, pattern)
			
		if pattern isa RELiteralExpression:
			return ExpandRegexPattern(matchValue, pattern)
			
		binary = pattern as BinaryExpression
		if binary is not null:
			if BinaryOperatorType.Assign == binary.Operator:
				return ExpandCapturePattern(matchValue, pattern)
				
			if BinaryOperatorType.BitwiseOr == binary.Operator:
				return ExpandEitherPattern(matchValue, pattern)
				
			if BinaryOperatorType.BitwiseAnd == binary.Operator:
				return ExpandBothPattern(matchValue, pattern)
	
			if BinaryOperatorType.And == binary.Operator:
				return ExpandConstrainedAndPattern(matchValue, pattern)
				
			if BinaryOperatorType.Or == binary.Operator:
				return ExpandConstrainedOrPattern(matchValue, pattern)
				
		if pattern isa ArrayLiteralExpression:
			return ExpandFixedSizePattern(matchValue, pattern)
				
		return ExpandValuePattern(matchValue, pattern)
		
	def ExpandBothPattern(matchValue as Expression, node as BinaryExpression) as Expression:
		l = Expand(matchValue, node.Left)
		r = Expand(matchValue, node.Right)
		return [| $l and $r |]
		
	def ExpandConstrainedAndPattern(matchValue as Expression, node as BinaryExpression) as Expression:
		l = Expand(matchValue, node.Left)
		return [| $l and $(node.Right) |]
		
	def ExpandConstrainedOrPattern(matchValue as Expression, node as BinaryExpression) as Expression:
		l = Expand(matchValue, node.Left)
		return [| $l or $(node.Right) |]
		
	def ExpandRegexPattern(matchValue as Expression, node as RELiteralExpression):
		tempBinding = NewTemp(node)
		return ExpandRegexPatternWithBinding(matchValue, node, tempBinding)
		
	def ExpandRegexPatternWithBinding(matchValue as Expression, pattern as RELiteralExpression, binding as ReferenceExpression):
		
		groupNames = array[of string](groupName for groupName in pattern.Regex.GetGroupNames() if not IsInteger(groupName))
		
		expansion = [| __eval__($binding = $pattern.Match($matchValue)) |]
		if len(groupNames) == 0:
			expansion.Arguments.Add([| $binding.Success |])
		else:
			bindingAction = [| __eval__() |]
			for groupName in groupNames:
				groupBinding = ReferenceExpression(LexicalInfo: pattern.LexicalInfo, Name: groupName)
				bindingAction.Arguments.Add([| $groupBinding = $binding.Groups[$groupName].Captures |])
			bindingAction.Arguments.Add([| true |])
			
			expansion.Arguments.Add([| ($bindingAction if $binding.Success else false) |])
			
		return expansion
	
	def IsInteger(s as string):
		_ as int
		return int.TryParse(s, _) 
		
	def ExpandEitherPattern(matchValue as Expression, node as BinaryExpression) as Expression:
		l = Expand(matchValue, node.Left)
		r = Expand(matchValue, node.Right)
		return [| $l or $r |]
		
	def ExpandBindPattern(matchValue as Expression, node as ReferenceExpression):
		return [| __eval__($node = $matchValue, true) |]
		
	def ExpandValuePattern(matchValue as Expression, node as Expression):
		return [| $matchValue == $node |]
		
	def ExpandCapturePattern(matchValue as Expression, node as BinaryExpression):
		name = node.Left
		pattern = node.Right
		if pattern isa RELiteralExpression:
			return ExpandRegexPatternWithBinding(matchValue, pattern, name)
			
		return ExpandObjectPattern(matchValue, name, pattern)
		
	def ExpandObjectPattern(matchValue as Expression, node as MethodInvocationExpression) as Expression:
		if len(node.NamedArguments) == 0 and len(node.Arguments) == 0:
			return [| $matchValue isa $(TypeRefFrom(node)) |]
		return ExpandObjectPattern(matchValue, NewTemp(node), node)

	def ExpandObjectPattern(matchValue as Expression, temp as ReferenceExpression, node as MethodInvocationExpression) as Expression:
		
		condition = [| ($matchValue isa $(TypeRefFrom(node))) and __eval__($temp = cast($(TypeRefFrom(node)), $matchValue), true) |]
		condition.LexicalInfo = node.LexicalInfo
		
		for member in node.Arguments:
			assert member isa ReferenceExpression, "Invalid argument '${member}' in pattern '${node}'."
			memberRef = MemberReferenceExpression(member.LexicalInfo, temp.CloneNode(), member.ToString())
			condition = [| $condition and __eval__($member = $memberRef, true) |]  
			
		for member in node.NamedArguments:
			namedArgCondition = ExpandMemberPattern(temp.CloneNode(), member)
			condition = [| $condition and $namedArgCondition |]
			
		return condition
	
	class QuasiquotePatternBuilder(DepthFirstVisitor):
		
		static final Ast = [| Boo.Lang.Compiler.Ast |]
		
		_parent as PatternExpander
		_pattern as Expression
		
		def constructor(parent as PatternExpander):
			_parent = parent
		
		def Build(node as QuasiquoteExpression):
			return Expand(node.Node)
			
		def Expand(node as Node):
			node.Accept(self)
			expansion = _pattern
			_pattern = null
			assert expansion is not null, "Unsupported pattern '${node}'"
			return expansion
			
		def Push(srcNode as Node, e as Expression):
			assert _pattern is null
			e.LexicalInfo = srcNode.LexicalInfo
			_pattern = e
			
		override def OnSpliceExpression(node as SpliceExpression):
			_pattern = node.Expression
			
		override def OnSpliceTypeReference(node as SpliceTypeReference):
			_pattern = node.Expression
			
		def ExpandFixedSize(items):
			a = [| (,) |]
			for item in items:
				a.Items.Add(Expand(item))
			return a
			
		override def OnOmittedExpression(node as OmittedExpression):
			_pattern = [| $Ast.OmittedExpression.Instance |]
			
		override def OnSlice(node as Slice):
			ctor = [| $Ast.Slice() |]
			ExpandProperty ctor, "Begin", node.Begin
			ExpandProperty ctor, "End", node.End
			ExpandProperty ctor, "Step", node.Step
			Push node, ctor
			
		def ExpandProperty(ctor as MethodInvocationExpression, name as string, value as Expression):
			if value is null: return
			ctor.NamedArguments.Add(ExpressionPair(First: ReferenceExpression(name), Second: Expand(value)))
			
		override def OnMacroStatement(node as MacroStatement):
			if len(node.Arguments) > 0:
				Push node, [| $Ast.MacroStatement(Name: $(node.Name), Arguments: $(ExpandFixedSize(node.Arguments))) |]
			else:
				Push node, [| $Ast.MacroStatement(Name: $(node.Name)) |]
			
		override def OnSlicingExpression(node as SlicingExpression):
			Push node, [| $Ast.SlicingExpression(Target: $(Expand(node.Target)), Indices: $(ExpandFixedSize(node.Indices))) |]
			
		override def OnTryCastExpression(node as TryCastExpression):
			Push node, [| $Ast.TryCastExpression(Target: $(Expand(node.Target)), Type: $(Expand(node.Type))) |]
			
		override def OnMethodInvocationExpression(node as MethodInvocationExpression):
			if len(node.Arguments) > 0:
				pattern = [| $Ast.MethodInvocationExpression(Target: $(Expand(node.Target)), Arguments: $(ExpandFixedSize(node.Arguments))) |]
			else:
				pattern = [| $Ast.MethodInvocationExpression(Target: $(Expand(node.Target))) |]
			Push node, pattern
			
		override def OnBoolLiteralExpression(node as BoolLiteralExpression):
			Push node, [| $Ast.BoolLiteralExpression(Value: $node) |]
			
		override def OnNullLiteralExpression(node as NullLiteralExpression):
			Push node, [| $Ast.NullLiteralExpression() |]
			
		override def OnUnaryExpression(node as UnaryExpression):
			Push node, [| $Ast.UnaryExpression(Operator: UnaryOperatorType.$(node.Operator.ToString()), Operand: $(Expand(node.Operand))) |]
			
		override def OnBinaryExpression(node as BinaryExpression):
			Push node, [| $Ast.BinaryExpression(Operator: BinaryOperatorType.$(node.Operator.ToString()), Left: $(Expand(node.Left)), Right: $(Expand(node.Right))) |]
		
		override def OnReferenceExpression(node as ReferenceExpression):
			Push node, [| $Ast.ReferenceExpression(Name: $(node.Name)) |]
			
		override def OnSuperLiteralExpression(node as SuperLiteralExpression):
			Push node, [| $Ast.SuperLiteralExpression() |]
			
	def ObjectPatternFor(node as QuasiquoteExpression):
		return QuasiquotePatternBuilder(self).Build(node)
		
	def ExpandQuasiquotePattern(matchValue as Expression, node as QuasiquoteExpression) as Expression:
		return ExpandObjectPattern(matchValue, ObjectPatternFor(node))
		
	def ExpandMemberPattern(matchValue as Expression, member as ExpressionPair):
		memberRef = MemberReferenceExpression(member.First.LexicalInfo, matchValue, member.First.ToString())	
		return Expand(memberRef, member.Second)
		
	def ExpandFixedSizePattern(matchValue as Expression, pattern as ArrayLiteralExpression):
		patternLen = len(pattern.Items)
		condition = [| $(patternLen) == len($matchValue) |]

		if IsCatchAllPattern(last = pattern.Items[patternLen-1]):
			pattern.Items.Remove(last)
			condition = [| $(patternLen) <= len($matchValue)+1 |]

		i = 0
		for item in pattern.Items:
			itemValue = [| $matchValue[$i] |]
			itemPattern = Expand(itemValue, item)
			condition = [| $condition and $itemPattern |]
			++i
		return condition
		
	def TypeRefFrom(node as MethodInvocationExpression):
		return node.Target

	def IsCatchAllPattern(pattern as Expression) as bool:
		last = pattern as UnaryExpression
		if AstUtil.IsExplodeExpression(last):
			last_re = last.Operand as ReferenceExpression
			return last_re and last_re.Name == Dummy
		return false


internal def NewTemp(e as Expression):
	return ReferenceExpression(
			LexicalInfo: e.LexicalInfo,
			Name: CompilerContext.Current.GetUniqueName("match"))

